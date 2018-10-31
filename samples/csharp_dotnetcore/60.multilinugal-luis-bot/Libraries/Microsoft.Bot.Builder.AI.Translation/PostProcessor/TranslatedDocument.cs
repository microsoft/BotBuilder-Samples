// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// Translated document is the data object holding all information of the translator module output on an input string.
    /// </summary>
    public class TranslatedDocument : ITranslatedDocument
    {
        private string sourceMessage;
        private string translatedMessage;
        private string rawAlignment;
        private Dictionary<int, int> indexedAlignment;
        private string[] sourceTokens;
        private string[] translatedTokens;
        private HashSet<string> literanlNoTranslatePhrases;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatedDocument"/> class  using only source message.
        /// </summary>
        /// <param name="sourceMessage">Source message.</param>
        public TranslatedDocument(string sourceMessage)
        {
            if (string.IsNullOrWhiteSpace(sourceMessage))
            {
                throw new ArgumentNullException(nameof(sourceMessage));
            }

            this.sourceMessage = sourceMessage;
            this.indexedAlignment = new Dictionary<int, int>();
            literanlNoTranslatePhrases = new HashSet<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatedDocument"/> class using source message and target/translated message.
        /// </summary>
        /// <param name="sourceMessage">Source message.</param>
        /// <param name="translatedMessage">Target/translated message.</param>
        public TranslatedDocument(string sourceMessage, string translatedMessage)
            : this(sourceMessage)
        {
            if (string.IsNullOrWhiteSpace(translatedMessage))
            {
                throw new ArgumentNullException(nameof(translatedMessage));
            }

            this.translatedMessage = translatedMessage;
        }

        public Dictionary<int, int> GetIndexedAlignment() => indexedAlignment;

        public HashSet<string> GetLiteranlNoTranslatePhrases() => literanlNoTranslatePhrases;

        public string GetRawAlignment() => rawAlignment;

        public string GetSourceMessage() => sourceMessage;

        public string[] GetSourceTokens() => sourceTokens;

        public string GetTranslatedMessage() => translatedMessage;

        public string[] GetTranslatedTokens() => translatedTokens;

        public void SetIndexedAlignment(Dictionary<int, int> indexedAlignment) => this.indexedAlignment = indexedAlignment;

        public void SetLiteranlNoTranslatePhrases(HashSet<string> literanlNoTranslatePhrases) => this.literanlNoTranslatePhrases = literanlNoTranslatePhrases;

        public void SetRawAlignment(string rawAlignment) => this.rawAlignment = rawAlignment;

        public void SetSourceMessage(string sourceMessage) => this.sourceMessage = sourceMessage;

        public void SetSourceTokens(string[] sourceTokens) => this.sourceTokens = sourceTokens;

        public void SetTranslatedMessage(string translatedMessage) => this.translatedMessage = translatedMessage;

        public void SetTranslatedTokens(string[] translatedTokens) => this.translatedTokens = translatedTokens;
    }
}
