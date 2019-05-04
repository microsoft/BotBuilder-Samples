// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Bot.Builder.AI.Translation.PostProcessor
{
    /// <summary>
    /// Custom dictionary post processor is used to forcibly translate certain vocab from a provided user dictionary.
    /// </summary>
    public class ConfiguredLanguageDictionaryPostProcessor : IPostProcessor
    {
        private readonly ConfiguredLanguageDictionary _useruserConfiguredLanguageDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguredLanguageDictionaryPostProcessor"/> class.
        /// </summary>
        /// <param name="userConfiguredLanguageDictionary">A <see cref="ConfiguredLanguageDictionary"/> object that stores all the different languages dictionaries keyed by language id.</param>
        public ConfiguredLanguageDictionaryPostProcessor(ConfiguredLanguageDictionary userConfiguredLanguageDictionary)
        {
            this._useruserConfiguredLanguageDictionary = userConfiguredLanguageDictionary ?? throw new ArgumentNullException(nameof(userConfiguredLanguageDictionary));
        }

        /// <summary>
        /// Process the logic for user configured language dictionary post processor used to handle user custom vocab translation.
        /// </summary>
        /// <param name="translatedDocument">Translated document.</param>
        /// <param name="languageId">Current source language id.</param>
        /// <returns>A <see cref="PostProcessedDocument"/> stores the original translated document state and the newly post processed message.</returns>
        public PostProcessedDocument Process(ITranslatedDocument translatedDocument, string languageId)
        {
            // Check if provided custom dictionary for this language is not empty
            if (_useruserConfiguredLanguageDictionary.GetLanguageDictionary(languageId).Count > 0)
            {
                string processedResult;
                var languageDictionary = _useruserConfiguredLanguageDictionary.GetLanguageDictionary(languageId);

                // Loop for all the original message tokens, and check if any of these tokens exists in the user custom dictionary,
                // to forcibly overwrite this token's translation with the user provided translation
                for (var i = 0; i < translatedDocument.GetSourceTokens().Length; i++)
                {
                    if (languageDictionary.ContainsKey(translatedDocument.GetSourceTokens()[i]))
                    {
                        // If a token of the original source message/phrase found in the user dictionary,
                        // replace it's equivalent translated token with the user provided translation
                        // the equivalent translated token can be found using the alignment map in the translated document
                        translatedDocument.GetTranslatedTokens()[translatedDocument.GetIndexedAlignment()[i]] = languageDictionary[translatedDocument.GetSourceTokens()[i]];
                    }
                }

                // Finally return PostProcessedDocument object that holds the orignal TRanslatedDocument and a string that joins all the translated tokens together
                processedResult = PostProcessingUtilities.Join(" ", translatedDocument.GetTranslatedTokens());
                return new PostProcessedDocument(translatedDocument, processedResult);
            }
            else
            {
                return new PostProcessedDocument(translatedDocument, string.Empty);
            }
        }
    }
}
