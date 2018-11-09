// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.Translation.Model
{
    /// <summary>
    /// An object with a single string property named proj, which maps input text to translated text.
    /// Alignment is returned as a string value of the following format:
    /// <c>[[SourceTextStartIndex]:[SourceTextEndIndex]–[TgtTextStartIndex]:[TgtTextEndIndex]]</c>.
    /// The colon separates start and end index, the dash separates the languages,
    /// and space separates the words. One word may align with zero, one, or multiple words
    /// in the other language, and the aligned words may be non-contiguous. When no alignment
    /// information is available, the alignment element will be empty.
    /// </summary>
    internal class Alignment
    {
        [JsonProperty("proj")]
        public string Projection { get; set; }
    }
}
