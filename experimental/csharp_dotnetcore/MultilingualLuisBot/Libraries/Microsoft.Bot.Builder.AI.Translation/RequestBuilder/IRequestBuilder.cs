// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Bot.Builder.AI.Translation.Model;

namespace Microsoft.Bot.Builder.AI.Translation.RequestBuilder
{
    /// <summary>
    /// Defines interface for HttpRequest builder.
    /// </summary>
    internal interface IRequestBuilder
    {
        /// <summary>
        /// Build the HttpRequestMessage for translation.
        /// </summary>
        /// <param name="sourceLanguage">The language to translate from.</param>
        /// <param name="targetLanguage">The language to translate to.</param>
        /// <param name="translatorRequests">The requests to be translated.</param>
        /// <returns>An HttpRequestMessage for the translator API.</returns>
        HttpRequestMessage BuildTranslateRequest(string sourceLanguage, string targetLanguage, IEnumerable<TranslatorRequestModel> translatorRequests);

        /// <summary>
        /// Builds the HttpRequestMessage for language detection.
        /// </summary>
        /// <param name="detectorRequests">The requests to be detected.</param>
        /// <returns>An HttpRequestMessage for the detection API.</returns>
        HttpRequestMessage BuildDetectRequest(IEnumerable<TranslatorRequestModel> detectorRequests);
    }
}
