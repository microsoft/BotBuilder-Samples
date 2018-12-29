// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.Translation.Model
{
    /// <summary>
    /// Detected language with Translator API v3.
    /// </summary>
    internal class DetectedLanguageModel
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }
    }
}
