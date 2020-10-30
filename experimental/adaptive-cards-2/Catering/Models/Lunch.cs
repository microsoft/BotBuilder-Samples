// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;

namespace Catering.Models
{
    public class Lunch
    {
        [JsonProperty("orderTimestamp")]
        public DateTime OrderTimestamp { get; set; }

        [JsonProperty("drink")]
        public string Drink { get; set; }

        [JsonProperty("entre")]
        public string Entre { get; set; }
    }
}
