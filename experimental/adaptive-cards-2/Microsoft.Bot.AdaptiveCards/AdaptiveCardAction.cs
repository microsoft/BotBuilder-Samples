// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Newtonsoft.Json;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardAction
    {
        public const string Name = "adaptiveCard/action";

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
