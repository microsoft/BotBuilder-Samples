// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.AI.CLU
{
    public class CluEntity
    {
        [JsonProperty("category")]
        public string category { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("offset")]
        public int offset { get; set; }
        [JsonProperty("length")]
        public int length { get; set; }
        [JsonProperty("confidenceScore")]
        public double confidenceScore { get; set; }
    }
}
