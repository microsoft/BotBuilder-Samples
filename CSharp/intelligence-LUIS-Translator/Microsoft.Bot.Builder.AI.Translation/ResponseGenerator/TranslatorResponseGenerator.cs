using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Newtonsoft.Json;
[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.AI.Translation.Tests")]

namespace Microsoft.Bot.Builder.AI.Translation.ResponseGenerator
{
    internal class TranslatorResponseGenerator : IResponseGenerator
    {
        private static readonly HttpClient DefaultHttpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(20) };
        private HttpClient _httpClient = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorResponseGenerator"/> class.
        /// </summary>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        public TranslatorResponseGenerator(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? DefaultHttpClient;
        }

        public async Task<IEnumerable<DetectedLanguageModel>> GenerateDetectResponseAsync(HttpRequestMessage request)
        {
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var detectedLanguages = JsonConvert.DeserializeObject<IEnumerable<DetectedLanguageModel>>(result);
                    return detectedLanguages;
                }
                else
                {
                    var errorResult = JsonConvert.DeserializeObject<ErrorModel>(result);
                    throw new ArgumentException(errorResult.Error.Message);
                }
            }
        }

        public async Task<IEnumerable<TranslatedResult>> GenerateTranslateResponseAsync(HttpRequestMessage request)
        {
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var translatedResults = JsonConvert.DeserializeObject<IEnumerable<TranslatedResult>>(responseBody);
                    return translatedResults;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var errorResult = JsonConvert.DeserializeObject<ErrorModel>(responseBody);
                    throw new ArgumentException(errorResult.Error.Message);
                }
            }
        }
    }
}
