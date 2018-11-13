// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.Model;

namespace Microsoft.Bot.Builder.AI.Translation.ResponseGenerator
{
    /// <summary>
    /// Defines interface for extracting models from http requests.
    /// </summary>
    internal interface IResponseGenerator
    {
        /// <summary>
        /// Generate the DetectedLanguageModel from the detection request.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to use.</param>
        /// <returns>A task that represents the generation operation.
        /// The task result contains the detected language models.</returns>
        Task<IEnumerable<DetectedLanguageModel>> GenerateDetectResponseAsync(HttpRequestMessage request);

        /// <summary>
        /// Generates the TranslatedResult from translation request.
        /// </summary>
        /// <param name="request">The HttpRequestMessage to use.</param>
        /// <returns>A task that represents the generation operation.
        /// The task result contains the translation results.</returns>
        Task<IEnumerable<TranslatedResult>> GenerateTranslateResponseAsync(HttpRequestMessage request);
    }
}
