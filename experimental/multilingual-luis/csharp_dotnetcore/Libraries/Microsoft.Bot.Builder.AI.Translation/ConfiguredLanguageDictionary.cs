// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// A Custom dictionary used to store all the configured user language dictionaries
    /// which in turn will be used in <see cref="CustomDictionaryPostProcessor"/> to overwrite the machine translation output for specific vocab
    /// with the specific translation in the provided dictionary,
    /// the <see cref="ConfiguredLanguageDictionary"/> contains an internal dictionary of dictionaries indexed by language id,
    /// for ex of how this interal state would look like :
    /// [
    ///     "en", ["court", "courtyard"]
    ///     "it", ["camera", "bedroom"]
    /// ]
    /// as per the last example, the outer dictionary contains all the user configured custom dictionaries indexed by the language id,
    /// and each internal dictionary contains the <see cref="KeyValuePair{string, string}"/> of this specific language.
    /// </summary>
    public class ConfiguredLanguageDictionary
    {
        private readonly Dictionary<string, Dictionary<string, string>> _userCustomDictionaries = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Adds new custom language dictionary for the set of configured dictionaries.
        /// </summary>
        /// <param name="languageId">The language id to use as the key of the new custom language dictionary.</param>
        /// <param name="dictionary">The dictionary containing the <see cref="KeyValuePair{string, string}"/> of the specific language.</param>
        public void AddNewLanguageDictionary(string languageId, Dictionary<string, string> dictionary)
        {
            if (string.IsNullOrWhiteSpace(languageId))
            {
                throw new ArgumentNullException(nameof(languageId));
            }

            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (_userCustomDictionaries.ContainsKey(languageId))
            {
                throw new ArgumentException(MessagesProvider.ExistingDictionaryErrorMessage);
            }

            _userCustomDictionaries.Add(languageId, dictionary);
        }

        /// <summary>
        /// Get a specific language dictionary using it's key (language id).
        /// </summary>
        /// <param name="languageId">The id of the language dictionary to get.</param>
        /// <returns>A <see cref="Dictionary{string, string}"/> that matches the provided language id.</returns>
        public Dictionary<string, string> GetLanguageDictionary(string languageId)
        {
            if (string.IsNullOrWhiteSpace(languageId))
            {
                throw new ArgumentNullException(nameof(languageId));
            }

            if (!_userCustomDictionaries.ContainsKey(languageId))
            {
                throw new KeyNotFoundException($"{MessagesProvider.NonExistentDictionaryErrorMessage} : {languageId}");
            }

            return _userCustomDictionaries[languageId];
        }

        /// <summary>
        /// Check if the <see cref="ConfiguredLanguageDictionary"/> object is empty or not.
        /// </summary>
        /// <returns><c>true</c> if user custom dictionary is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty() => _userCustomDictionaries.Count == 0;
    }
}
