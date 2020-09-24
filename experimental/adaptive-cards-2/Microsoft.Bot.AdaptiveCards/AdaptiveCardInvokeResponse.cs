using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardInvokeResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
