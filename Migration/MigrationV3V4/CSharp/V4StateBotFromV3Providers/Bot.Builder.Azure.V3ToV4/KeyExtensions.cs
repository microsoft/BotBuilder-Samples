// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bot.Builder.Azure.V3V4
{
    /// <summary>
    /// Extension methods brought from Bot Builder V3 Azure.
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal static class KeyExtensions
    {
        private static readonly Dictionary<string, string> _DefaultReplacementsForCharactersDisallowedByAzure = new Dictionary<string, string>() { { "/", "|s|" }, { @"\", "|b|" }, { "#", "|h|" }, { "?", "|q|" } };

        /// <summary>
        /// Replaces the four characters disallowed in Azure keys with something more palatable.  You can provide your own mapping if you don't like the defaults.
        /// </summary>
        internal static string SanitizeForAzureKeys(this string input, Dictionary<string, string> replacements = null)
        {
            var repmap = replacements ?? _DefaultReplacementsForCharactersDisallowedByAzure;
            return input.Trim().Replace("/", repmap["/"]).Replace(@"\", repmap[@"\"]).Replace("#", repmap["#"]).Replace("?", repmap["?"]);
        }

        private static string TruncateEntityKey(string entityKey)
        {
            const int MAX_KEY_LENGTH = 254;
            if (entityKey.Length > MAX_KEY_LENGTH)
            {
                var hash = entityKey.GetHashCode().ToString("x");
                entityKey = entityKey.Substring(0, MAX_KEY_LENGTH - hash.Length) + hash;
            }

            return entityKey;
        }

        internal static string SanitizeTableName(this string input)
        {
            if (input.Length > 63)
            {
                input = input.Substring(0, 63);
            }
            input = Regex.Replace(input, @"[^a-zA-Z0-9]+", "");
            return input;
        }
    }
}
