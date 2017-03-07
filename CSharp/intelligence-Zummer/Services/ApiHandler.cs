using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Zummer.Services
{
    /// <summary>
    /// Responsible for constructing and issuing Http GET requests for certain API
    /// </summary>
    internal sealed class ApiHandler : IApiHandler
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<T> GetJsonAsync<T>(string url, IDictionary<string, string> requestParameters, IDictionary<string, string> headers) where T : class
        {
            string rawResponse = await this.SendRequestAsync(url, requestParameters, headers);

            return JsonConvert.DeserializeObject<T>(rawResponse);
        }

        public async Task<string> GetStringAsync(string url, IDictionary<string, string> requestParameters, IDictionary<string, string> headers)
        {
            return await this.SendRequestAsync(url, requestParameters, headers);
        }

        private async Task<string> SendRequestAsync(string url, IDictionary<string, string> requestParameters, IDictionary<string, string> headers)
        {
            this.client.DefaultRequestHeaders.Clear();
            
            string fullUrl = url;

            if (requestParameters != null)
            {
                var requestParams = "?";
                bool first = true;

                foreach (var elem in requestParameters)
                {
                    requestParams += (first == false ? "&" : string.Empty) + $"{elem.Key}={HttpUtility.UrlEncode(elem.Value)}";
                    first = false;
                }

                fullUrl += requestParams;
            }


            if (headers != null)
            {
                foreach (var header in headers)
                {
                    this.client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            
            var response = await this.client.GetAsync(fullUrl);
            var rawResponse = await response.Content.ReadAsStringAsync();

            return rawResponse;
        }
    }
}