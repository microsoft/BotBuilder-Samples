// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

        /// <summary>
        /// Generate the DetectedLanguageModel from the detection request.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to use.</param>
        /// <returns>A task that represents the generation operation.
        /// The task result contains the detected language models.</returns>
        public async Task<IEnumerable<DetectedLanguageModel>> GenerateDetectResponseAsync(HttpRequestMessage request)
        {
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                if (response != null)
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
                else
                {
                    throw new ArgumentNullException(string.Format("{0}. Received a null {1}.", MessagesProvider.NullResponseErrorMessage, nameof(HttpRequestMessage)));
                }
            }
        }

        /// <summary>
        /// Generates the TranslatedResult from translation request.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to use.</param>
        /// <returns>A task that represents the generation operation.
        /// The task result contains the translation results.</returns>
        public async Task<IEnumerable<TranslatedResult>> GenerateTranslateResponseAsync(HttpRequestMessage request)
        {
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                if (response != null)
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var translatedResults = JsonConvert.DeserializeObject<IEnumerable<TranslatedResult>>(responseBody);
                        return translatedResults;
                    }
                    else
                    {
                        var errorResult = JsonConvert.DeserializeObject<ErrorModel>(responseBody);
                        throw new ArgumentException(errorResult.Error.Message);
                    }
                }
                else
                {
                    throw new ArgumentNullException(string.Format("{0}. Received a null {1}.", MessagesProvider.NullResponseErrorMessage, nameof(HttpRequestMessage)));
                }
            }
        }
    }
}
