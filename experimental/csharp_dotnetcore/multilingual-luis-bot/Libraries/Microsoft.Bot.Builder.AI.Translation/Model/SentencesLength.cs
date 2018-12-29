// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.Translation.Model
{
    /// <summary>
    /// Represents the sentence boundaries in the input and output texts.
    /// </summary>
    internal class SentencesLength
    {
        [JsonProperty("srcSentLen")]
        public IEnumerable<int> Source { get; set; }

        [JsonProperty("transSentLen")]
        public IEnumerable<int> Translation { get; set; }
    }
}
