// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// Defines interface for Microsoft Translator Text API.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Detects the language of the input text.
        /// </summary>
        /// <param name="textToDetect">The text to translate.</param>
        /// <returns>A task that represents the detection operation.
        /// The task result contains the id of the detected language.</returns>
        Task<string> DetectAsync(string textToDetect);

        /// <summary>
        /// Translates a single message from a source language to a target language.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <param name="sourceLanguage">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="targetLanguage">The language code to translate the text into.</param>
        /// <returns>A task that represents the translation operation.
        /// The task result contains the translated document.</returns>
        Task<ITranslatedDocument> TranslateAsync(string textToTranslate, string sourceLanguage, string targetLanguage);

        /// <summary>
        /// Translates an array of strings from a source language to a target language.
        /// </summary>
        /// <param name="translateArraySourceTexts">The strings to translate.</param>
        /// <param name="sourceLanguage">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="targetLanguage">The language code to translate the text into.</param>
        /// <returns>A task that represents the translation operation.
        /// The task result contains a list of the translated documents.</returns>
        Task<List<ITranslatedDocument>> TranslateArrayAsync(string[] translateArraySourceTexts, string sourceLanguage, string targetLanguage);
    }
}
