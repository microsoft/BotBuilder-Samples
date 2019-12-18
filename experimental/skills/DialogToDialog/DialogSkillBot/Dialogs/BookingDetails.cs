// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.Dialogs
{
    public class BookingDetails
    {
        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("travelDate")]
        public string TravelDate { get; set; }
    }
}
