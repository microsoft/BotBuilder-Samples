// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Newtonsoft.Json;
[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.AI.Translation.Tests")]

namespace Microsoft.Bot.Builder.AI.Translation.RequestBuilder
{
    /// <summary>
    /// Provides http requests needed for translation and language detection.
    /// </summary>
    internal class TranslatorRequestBuilder : IRequestBuilder
    {
        // TODO: Make it configurable
        private const string DetectUrl = "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        private const string TranslateUrl = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true";
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorRequestBuilder"/> class.
        /// </summary>
        /// <param name="apiKey">Your subscription key for the Microsoft Translator Text API.</param>
        public TranslatorRequestBuilder(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            _apiKey = apiKey;
        }

        /// <summary>
        /// Build the HttpRequestMessage for translation.
        /// </summary>
        /// <param name="sourceLanguage">The language to translate from.</param>
        /// <param name="targetLanguage">The language to translate to.</param>
        /// <param name="translatorRequests">The requests to be translated.</param>
        /// <returns>An HttpRequestMessage for the translator API.</returns>
        public HttpRequestMessage BuildTranslateRequest(string sourceLanguage, string targetLanguage, IEnumerable<TranslatorRequestModel> translatorRequests)
        {
            if (translatorRequests == null || translatorRequests.ToList().Count < 1)
            {
                throw new ArgumentNullException(nameof(translatorRequests));
            }

            var query = $"&from={sourceLanguage}&to={targetLanguage}";
            var requestUri = new Uri(TranslateUrl + query);
            return GetRequestMessage(requestUri, translatorRequests);
        }

        /// <summary>
        /// Builds the HttpRequestMessage for language detection.
        /// </summary>
        /// <param name="detectorRequests">The requests to be detected.</param>
        /// <returns>An HttpRequestMessage for the detection API.</returns>
        public HttpRequestMessage BuildDetectRequest(IEnumerable<TranslatorRequestModel> detectorRequests)
        {
            if (detectorRequests == null || detectorRequests.ToList().Count < 1)
            {
                throw new ArgumentNullException(nameof(detectorRequests));
            }

            var requestUri = new Uri(DetectUrl);
            return GetRequestMessage(requestUri, detectorRequests);
        }

        /// <summary>
        /// Build HttpRequestMessage with its content.
        /// </summary>
        /// <param name="requestUri">Uri of request</param>
        /// <param name="translatorRequests">The models to be included in the content.</param>
        /// <returns>An HttpRequestMessage with its content.</returns>
        private HttpRequestMessage GetRequestMessage(Uri requestUri, IEnumerable<TranslatorRequestModel> translatorRequests)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(translatorRequests), Encoding.UTF8, "application/json");
            return request;
        }
    }
}
