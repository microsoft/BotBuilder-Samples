using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
