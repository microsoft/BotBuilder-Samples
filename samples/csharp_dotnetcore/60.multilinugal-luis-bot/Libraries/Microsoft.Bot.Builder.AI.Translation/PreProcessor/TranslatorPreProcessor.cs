// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        private readonly string _literalPattern = "<literal>(.*)</literal>";

        public void PreprocessMessage(string textToTranslate, out string processedTextToTranslate, out HashSet<string> noTranslatePhrases)
        {
            textToTranslate = RemoveSpaces(textToTranslate);
            noTranslatePhrases = new HashSet<string>();
            var literalMatches = Regex.Matches(textToTranslate, _literalPattern);
            if (literalMatches.Count > 0)
            {
                noTranslatePhrases = BuildNoTranslatePhrases(literalMatches);
                textToTranslate = Regex.Replace(textToTranslate, "</?literal>", " ");
            }

            processedTextToTranslate = RemoveSpaces(textToTranslate);
        }

        public string PreprocessMessage(string textToTranslate)
        {
            textToTranslate = RemoveSpaces(textToTranslate);
            var literalMatches = Regex.Matches(textToTranslate, _literalPattern);
            if (literalMatches.Count > 0)
            {
                textToTranslate = Regex.Replace(textToTranslate, "</?literal>", " ");
            }

            return RemoveSpaces(textToTranslate);
        }

        private string RemoveSpaces(string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }

        private HashSet<string> BuildNoTranslatePhrases(MatchCollection literalMatches)
        {
            HashSet<string> noTranslatePhrases = new HashSet<string>();
            foreach (Match literalMatch in literalMatches)
            {
                if (literalMatch.Groups.Count > 1)
                {
                    noTranslatePhrases.Add(string.Format("({0})", literalMatch.Groups[1].Value));
                }
            }

            return noTranslatePhrases;
        }
    }
}
