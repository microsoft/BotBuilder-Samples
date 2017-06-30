using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;

namespace Microsoft.LUIS.API
{
    public class Application
    {
        private readonly Subscription _subscription;
        public readonly string ApplicationID;
        private readonly string _version;
        private readonly JObject _model;
        private readonly int? _take;
        private const int TooManyRequests = 429;
        private const int MaxRetry = 300;
        private const int MaxPageSize = 500;

        internal Application(Subscription subscription, JObject model, int? take = MaxPageSize)
        {
            _subscription = subscription;
            _model = model;
            ApplicationID = (string)model["id"];
            _version = "0.1";
            _take = take;
        }

        private IEnumerablePage<T> EnumerablePage<T>(string api, CancellationToken ct, int? take = null)
        {
            return new EnumerableAsync<T>(async (skip) =>
            {
                ICollection<T> result = null;
                var uri = $"{api}?skip={skip}";
                take = take ?? _take;
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

        public string AppAPI(string api)
        {
            return $"apps/{ApplicationID}/versions/{_version}/{api}";
        }

        private async Task<HttpResponseMessage> Retry(Func<Task<HttpResponseMessage>> func)
        {
            HttpResponseMessage response = null;
            int retries = 0;
            do
            {
                response = await func();
            } while ((int)response.StatusCode == TooManyRequests && ++retries < MaxRetry);
            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(string api, CancellationToken ct)
        {
            return await Retry(async () => await _subscription.GetAsync(AppAPI(api), ct));
        }

        public async Task<HttpResponseMessage> PostAsync(string api, JToken json, CancellationToken ct)
        {
            return await Retry(async () => await _subscription.PostAsync(AppAPI(api), json, ct));
        }

        public async Task<HttpResponseMessage> PutAsync(string api, JToken json, CancellationToken ct)
        {
            return await Retry(async () => await _subscription.PutAsync(AppAPI(api), json, ct));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string api, CancellationToken ct)
        {
            return await Retry(async () => await _subscription.DeleteAsync(AppAPI(api), ct));
        }

        /// <summary>
        /// Delete the application.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if application was deleted.</returns>
        public async Task<bool> DeleteAsync(CancellationToken ct)
        {
            var response = await _subscription.DeleteAsync($"apps/{ApplicationID}", ct);
            return response.IsSuccessStatusCode;
        }

        public string GetModelName(string id, CancellationToken ct)
        {
            string name = null;
            var models = EnumerablePage<dynamic>("models", ct);
            foreach(var model in models)
            {
                if (model.id == id)
                {
                    name = (string)model.name;
                    break;
                }
            }
            return name;
        }

        public enum TrainingStatus
        {
            Success = 0,
            Fail = 1,
            UpToDate = 2,
            InProgress = 3,
            Queued = 9
        };

        public async Task<bool> TrainAsync(CancellationToken ct)
        {
            var response = await PostAsync("train", new JObject(), ct);
            if (response.IsSuccessStatusCode)
            {
                bool isTrained = false;
                do
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    var a = JArray.Parse(await (await GetAsync("train", ct)).Content.ReadAsStringAsync());
                    isTrained = true;
                    foreach (dynamic model in a)
                    {
                        var status = model.details.statusId;
                        if (status == TrainingStatus.Fail)
                        {
                            var id = (string) model.modelId;
                            var name = GetModelName(id, ct);
                            throw new Exception($"Training failed for {name}: {model.details.failureReason}");
                        }
                        else if (status == TrainingStatus.InProgress || status == TrainingStatus.Queued)
                        {
                            isTrained = false;
                            break;
                        }
                    }
                } while (!isTrained);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PublishAsync(bool isStaging, CancellationToken ct)
        {
            var staging = isStaging ? "true" : "false";
            var body = JObject.Parse($"{{\"versionId\": \"{_version}\", \"isStaging\": {staging}}}");
            var response = await _subscription.PostAsync($"apps/{ApplicationID}/publish", body, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddSpelling(string key, CancellationToken ct)
        {
            return await AddExternalKey(JObject.Parse($@"{{""type"":""BingSpellCheck"", ""value"":""{key}""}}"), ct);
        }

        public async Task<JObject> DownloadAsync(CancellationToken ct)
        {
            var response = await GetAsync("export", ct);
            return response.IsSuccessStatusCode
                ? JObject.Parse(await response.Content.ReadAsStringAsync())
                : null;
        }

        public async Task<bool> AddExternalKey(dynamic key, CancellationToken ct)
        {
            var response = await PutAsync("externalKeys", key, ct);
            await _subscription.ThrowIfError(response);
            return true;
        }

        public async Task<bool> UploadUtteranceAsync(dynamic utterance, CancellationToken ct)
        {
            var response = await PostAsync("example", utterance, ct);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Upload a batch of labelled utterances.
        /// </summary>
        /// <param name="utterances">Utterances to upload.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task<JArray> UploadUtterancesAsync(dynamic utterances, CancellationToken ct)
        {
            var response = await PostAsync("examples", utterances, ct);
            return response.IsSuccessStatusCode
                ? JArray.Parse(await response.Content.ReadAsStringAsync())
                : null;
        }

        public IEnumerablePage<JObject> GetIntents(CancellationToken ct, int? take = null)
        {
            return EnumerablePage<JObject>("intents", ct, take);
        }

        public async Task<string> GetIntentIDAsync(string name, CancellationToken ct)
        {
            var intent = await GetIntents(ct).FirstAsync((i) => (string)i["name"] == name);
            return intent == null ? null : (string)intent["id"];
        }

        public async Task<bool> DeleteIntentAsync(string name, CancellationToken ct)
        {
            var success = true;
            var id = await GetIntentIDAsync(name, ct);
            if (id != null)
            {
                var response = await DeleteAsync($"intents/{id}", ct);
                success = response.IsSuccessStatusCode;
            }
            return success;
        }

        public async Task<bool> CreateIntentAsync(string name, CancellationToken ct)
        {
            var intent = new JObject();
            intent["name"] = name;
            var response = await PostAsync("intents", intent, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreatePhraseListAsync(dynamic phraseList, CancellationToken ct)
        {
            var response = await PostAsync("phraselists", phraseList, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<JArray> GetPhraseListsAsync(CancellationToken ct)
        {
            JArray result = null;
            var response = await GetAsync("phraselists", ct);
            if (response.IsSuccessStatusCode)
            {
                result = JArray.Parse(await response.Content.ReadAsStringAsync());
            }
            return result;
        }

        public async Task<string> GetPhraseListIDAsync(string name, CancellationToken ct)
        {
            string id = null;
            var phrases = await GetPhraseListsAsync(ct);
            if (phrases != null)
            {
                foreach (var phrase in phrases)
                {
                    if ((string)phrase["name"] == name)
                    {
                        id = (string)phrase["id"];
                        break;
                    }
                }
            }
            return id;
        }

        public async Task<bool> DeletePhraseListAsync(string name, CancellationToken ct)
        {
            var ok = true;
            var id = await GetPhraseListIDAsync(name, ct);
            if (id != null)
            {
                var response = await DeleteAsync($"phraselists/{id}", ct);
                ok = response.IsSuccessStatusCode;
            }
            return ok;
        }

        public async Task<JToken> QueryAsync(string query, CancellationToken ct, bool log = true, bool spellCheck = false, bool verbose = false)
        {
            var escQuery = Uri.EscapeDataString(query);
            var uri = $"https://{this._subscription.Domain}/luis/v2.0/apps/{ApplicationID}?subscription-key={_subscription.Key}&q={escQuery}";
            uri += $"&log={Uri.EscapeDataString(Convert.ToString(log))}&spellCheck={Uri.EscapeDataString(Convert.ToString(spellCheck))}&verbose={Uri.EscapeDataString(Convert.ToString(verbose))}";
            var response = await Retry(async () =>
            {
                return await _subscription.RawGetAsync(uri, ct);
            });
            JObject val = null;
            if (response.IsSuccessStatusCode)
            {
                val = JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            return val;
        }

        public IEnumerablePage<JObject> GetUtterances(CancellationToken ct, int? take = null)
        {
            return EnumerablePage<JObject>("examples", ct, take);
        }

        public async Task<IEnumerable<JObject>> GetUtterancesForIntents(IEnumerable<string> names, CancellationToken ct)
        {
            var set = new HashSet<string>(names);
            return await GetUtterances(ct).AllAsync((u) => set.Contains((string)u["intentLabel"]));
        }

        public async Task<bool> DeleteUtteranceAsync(JObject utterance, CancellationToken ct)
        {
            var id = (string)utterance["id"];
            var response = await DeleteAsync($"examples/{id}", ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> CreateClosedListAsync(dynamic closedList, CancellationToken ct)
        {
            var response = await PostAsync("closedlists", closedList, ct);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }
    }
}
