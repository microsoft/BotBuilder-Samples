// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.Dialogs
{
    public class Location
    {
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }
}
