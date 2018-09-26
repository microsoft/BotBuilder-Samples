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
        /// <param name="from">The language to translate from.</param>
        /// <param name="to">The language to translate to.</param>
        /// <param name="translatorRequests">The requests to be translated.</param>
        /// <returns>An HttpRequestMessage for the translator API.</returns>
        HttpRequestMessage GetTranslateRequestMessage(string from, string to, IEnumerable<TranslatorRequestModel> translatorRequests);

        /// <summary>
        /// Builds the HttpRequestMessage for language detection.
        /// </summary>
        /// <param name="detectorRequests">The requests to be detected.</param>
        /// <returns>An HttpRequestMessage for the detection API.</returns>
        HttpRequestMessage GetDetectRequestMessage(IEnumerable<TranslatorRequestModel> detectorRequests);
    }
}
