using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.AI.Translation.Tests")]

namespace Microsoft.Bot.Builder.AI.Translation.PreProcessor
{
    /// <summary>
    /// Provides preprocessing for text processed by the Translation API.
    /// </summary>
    internal class TranslatorPreProcessor : IPreProcessor
    {
        public TranslatorPreProcessor()
        {
        }

        public void PreprocessMessage(string textToTranslate, out string processedTextToTranslate, out HashSet<string> noTranslatePhrases)
        {
            textToTranslate = Regex.Replace(textToTranslate, @"\s+", " "); // used to remove multiple spaces in input user message
            var literalPattern = "<literal>(.*)</literal>";
            noTranslatePhrases = new HashSet<string>();
            var literalMatches = Regex.Matches(textToTranslate, literalPattern);
            if (literalMatches.Count > 0)
            {
                foreach (Match literalMatch in literalMatches)
                {
                    if (literalMatch.Groups.Count > 1)
                    {
                        noTranslatePhrases.Add("(" + literalMatch.Groups[1].Value + ")");
                    }
                }

                textToTranslate = Regex.Replace(textToTranslate, "</?literal>", " ");
            }

            textToTranslate = Regex.Replace(textToTranslate, @"\s+", " ");
            processedTextToTranslate = textToTranslate;
        }

        public string PreprocessMessage(string textToTranslate)
        {
            textToTranslate = Regex.Replace(textToTranslate, @"\s+", " "); // used to remove multiple spaces in input user message
            var literalPattern = "<literal>(.*)</literal>";
            var literalMatches = Regex.Matches(textToTranslate, literalPattern);
            if (literalMatches.Count > 0)
            {
                textToTranslate = Regex.Replace(textToTranslate, "</?literal>", " ");
            }

            return Regex.Replace(textToTranslate, @"\s+", " ");
        }
    }
}
