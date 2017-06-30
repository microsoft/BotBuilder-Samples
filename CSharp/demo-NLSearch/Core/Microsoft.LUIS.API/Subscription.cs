using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.LUIS.API
{
    public class Subscription
    {
        public readonly string Domain;
        public readonly string Key;
        private SemaphoreSlim _semaphore;
        private HttpClient _client;

        public Subscription(string domain, string subscription, int maxRequests = 30, string basicAuth = null)
        {
            this.Domain = domain;
            Key = subscription;
            _semaphore = new SemaphoreSlim(maxRequests, maxRequests);
            _client = new HttpClient();
            _client.Timeout = new TimeSpan(0, 2, 0);
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);
            if (basicAuth != null)
            {
                var byteArray = Encoding.ASCII.GetBytes(basicAuth);
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public Uri BaseUri(string api)
        {
            return _client.DefaultRequestHeaders.Contains("Authorization") 
                ? new Uri($"https://{Domain}/api/v2.0/{api}") 
                : new Uri($"https://{Domain}/luis/api/v2.0/{api}");
        }

        private async Task<HttpClient> ClientAsync()
        {
            await _semaphore.WaitAsync();
            return _client;
        }

        public async Task ThrowIfError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var msg = response.ReasonPhrase;
                try
                {
                    var text = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(text);
                    dynamic err = obj == null ? null : (JObject)obj["error"];
                    if (err != null)
                    {
                        msg = $"{err.code}: {err.message}";
                    }
                }
                finally
                {
                    throw new Exception(msg);
                }
            }
        }

        public async Task<HttpResponseMessage> RawGetAsync(string uri, CancellationToken ct)
        {
            var client = await ClientAsync();
            try
            {
                return await client.GetAsync(uri, ct);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string api, CancellationToken ct)
        {
            return await RawGetAsync(BaseUri(api).ToString(), ct);
        }

        private async Task<HttpResponseMessage> PutPostAsync(bool put, string api, JToken json, CancellationToken ct)
        {
            var client = await ClientAsync();
            try
            {
                var uri = BaseUri(api);
                HttpResponseMessage response;
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = put ? await client.PutAsync(uri, content, ct) : await client.PostAsync(uri, content, ct);
                }
                return response;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string api, JToken json, CancellationToken ct)
        {
            return await PutPostAsync(false, api, json, ct);
        }

        public async Task<HttpResponseMessage> PutAsync(string api, JToken json, CancellationToken ct)
        {
            return await PutPostAsync(true, api, json, ct);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string api, CancellationToken ct)
        {
            var client = await ClientAsync();
            try
            {
                var uri = BaseUri(api);
                return await client.DeleteAsync(uri, ct);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private IEnumerablePage<T> EnumerablePage<T>(string api, CancellationToken ct, int? take = null)
        {
            return new EnumerableAsync<T>(async (skip) =>
            {
                ICollection<T> result = null;
                var uri = $"{api}?skip={skip}";
                if (take.HasValue)
                {
                    uri += $"&take={take}";
                }
                var response = await GetAsync(uri, ct);
                if (response.IsSuccessStatusCode)
                {
                    var arr = JArray.Parse(await response.Content.ReadAsStringAsync());
                    if (arr.Count > 0)
                    {
                        result = (ICollection<T>)arr.Values<T>().ToList();
                    }
                }
                return result;
            }, ct);
        }

        /// <summary>
        /// Get all of the LUIS apps in subscription.
        /// </summary>
        /// <param name="subscription">LUIS subscription key.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>IEnumerablePage of app descriptions.</returns>
        public IEnumerablePage<JObject> GetApps(CancellationToken ct, int? take = null)
        {
            return EnumerablePage<JObject>("apps", ct, take);
        }

        public async Task<Application> GetApplicationAsync(string appID, CancellationToken ct)
        {
            var response = await GetAsync($"apps/{appID}", ct);
            return response.IsSuccessStatusCode
                ? new Application(this, JObject.Parse(await response.Content.ReadAsStringAsync()))
                : null;
        }

        /// <summary>
        /// Get a model by name.
        /// </summary>
        /// <param name="appName">Name of model.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>New application or null if not present.</returns>
        public async Task<Application> GetApplicationByNameAsync(string appName, CancellationToken ct)
        {
            var model = await GetApps(ct).FirstAsync((app) => string.Compare((string)app["name"], appName, true) == 0);
            return model == null ? null : new Application(this, model);
        }

        public async Task<bool> DeleteApplicationByNameAsync(string appName, CancellationToken ct)
        {
            bool deleted = false;
            var app = await GetApplicationByNameAsync(appName, ct);
            if (app != null)
            {
                deleted = await app.DeleteAsync(ct);
            }
            return deleted;
        }

        /// <summary>
        /// Import a LUIS model.
        /// </summary>
        /// <param name="appName">Name of app to upload.</param>
        /// <param name="model">LUIS model.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>ID of uploaded model.</returns>
        public async Task<Application> ImportApplicationAsync(string appName, JObject model, CancellationToken ct)
        {
            var response = await PostAsync($"apps/import?appName={appName}", model, ct);
            await ThrowIfError(response);
            var id = await response.Content.ReadAsStringAsync();
            return await GetApplicationAsync(id.Replace("\"", ""), ct);
        }

        public async Task<Application> ReplaceApplicationAsync(dynamic model, CancellationToken ct, string spellingKey = null)
        {
            Application newApp = null;
            string appName = (string)model.name;
            var old = await GetApplicationByNameAsync(appName, ct);
            if (old != null)
            {
                await old.DeleteAsync(ct);
            }
            Application app = null;
            try
            {
                app = await ImportApplicationAsync(appName, model, ct);
                if (app != null
                    && (spellingKey == null || await app.AddSpelling(spellingKey, ct))
                    && await app.TrainAsync(ct)
                    && await app.PublishAsync(false, ct))
                {
                    newApp = app;
                }
            }
            catch (Exception)
            {
                // Try to clean up non published model
                if (app != null)
                {
                    await app.DeleteAsync(ct);
                }
                throw;
            }
            return newApp;
        }

        public async Task<bool> AddExternalKey(dynamic  key, CancellationToken ct)
        {
            var response = await PostAsync("externalKeys", key, ct);
            await ThrowIfError(response);
            return true;
        }

        /// <summary>
        /// Return the LUIS application of an existing app or import it from <paramref name="modelPath"/> and return a new application.
        /// </summary>
        /// <param name="modelPath">Path to the exported LUIS model.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="spellingKey">Bing spelling key.</param>
        /// <returns>LUIS Model ID.</returns>
        public async Task<Application> GetOrImportApplicationAsync(string modelPath, CancellationToken ct, string spellingKey = null)
        {
            dynamic newModel = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(modelPath));
            var app = await GetApplicationByNameAsync((string)newModel.name, ct);
            if (app == null)
            {
                app = await ReplaceApplicationAsync(newModel, ct, spellingKey);
            }
            return app;
        }

        public async Task<Application> GetOrCreateApplicationAsync(string name, string culture, CancellationToken ct)
        {
            var app = await GetApplicationByNameAsync(name, ct);
            if (app == null)
            {
                var appDesc = new JObject();
                appDesc["name"] = name;
                appDesc["culture"] = culture;
                var response = await PostAsync("apps", appDesc, ct);
                if (response.IsSuccessStatusCode)
                {
                    var id = (await response.Content.ReadAsStringAsync()).Replace("\"", string.Empty);
                    app = await GetApplicationAsync(id, ct);
                }
            }
            return app;
        }
    }
}
