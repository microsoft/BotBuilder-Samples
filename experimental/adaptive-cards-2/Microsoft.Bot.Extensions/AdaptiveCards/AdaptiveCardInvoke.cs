// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardInvoke
    {
        [JsonProperty("action")]
        public AdaptiveCardAction Action { get; set; }

        [JsonProperty("authentication")]
        public AdaptiveCardAuthentication Authentication { get; set; }
    }
}
