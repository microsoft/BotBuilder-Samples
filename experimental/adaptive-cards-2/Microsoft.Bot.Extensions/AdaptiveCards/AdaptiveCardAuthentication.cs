// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardAuthentication
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("connectionName")]
        public string ConnectionName { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
