// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.AI.Translation.PreProcessor
{
    internal interface IPreProcessor
    {
        /// <summary>
        /// Performs pre-processing to remove "literal" tags and flag sections of the text that will not be translated.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <param name="processedTextToTranslate">The processed text after removing the literal tags and other unwanted characters.</param>
        /// <param name="noTranslatePhrases">The extracted no translate phrases.</param>
        void PreprocessMessage(string textToTranslate, out string processedTextToTranslate, out HashSet<string> noTranslatePhrases);

        /// <summary>
        /// Performs pre-processing to remove "literal" tags .
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <returns>A string represents the textToTranslate after preprocessing.</returns>
        string PreprocessMessage(string textToTranslate);
    }
}
