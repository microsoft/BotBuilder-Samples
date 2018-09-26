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
        /// <returns>The language identifier.</returns>
        Task<string> DetectAsync(string textToDetect);

        /// <summary>
        /// Translates a single message from a source language to a target language.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <param name="from">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="to">The language code to translate the text into.</param>
        /// <returns>The translated document.</returns>
        Task<ITranslatedDocument> TranslateAsync(string textToTranslate, string from, string to);

        /// <summary>
        /// Translates an array of strings from a source language to a target language.
        /// </summary>
        /// <param name="translateArraySourceTexts">The strings to translate.</param>
        /// <param name="from">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="to">The language code to translate the text into.</param>
        /// <returns>An array of the translated documents.</returns>
        Task<List<ITranslatedDocument>> TranslateArrayAsync(string[] translateArraySourceTexts, string from, string to);
    }
}
