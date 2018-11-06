// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// A class to store important messages that will return to the user as errors or logs.
    /// </summary>
    public class MessagesProvider
    {
        // error messages
        public const string ExistingDictionaryErrorMessage = "A language dictionary with the same language id already exists";
        public const string NonExistentDictionaryErrorMessage = "No dictionary found that matches the provided language id";
        public const string IncorrectAlignmentFormatErrorMessage = "Incorrect alignment format, please use the following format for each alignment entry : [starting_source_index]:[ending_source_index]-[starting_translated_index]:[ending_translated_index] ";
        public const string NegativeValueSourceWordIndexErrorMessage = "Source word index can't be negative";
        public const string NegativeValueAlignmentMapEntryErrorMessage = "Alignment map entry value can't be negative";
        public const string NotFoundTokenInSourceArray = "Token not found in the specified array, check the alignment information";
        public const string EmptyPatternsErrorMessage = "Patterns dictionary can't be empty";
        public const string NullResponseErrorMessage = "Failed to obtain a response from the API";

        // information messages
        public const string MissingTranslatorEnvironmentVariablesMessage = "Missing Translator Environment variables - Skipping test";
    }
}
