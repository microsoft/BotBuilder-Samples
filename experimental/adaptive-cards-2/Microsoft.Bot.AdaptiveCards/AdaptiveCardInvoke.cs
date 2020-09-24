using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Bot.AdaptiveCards
{
    public class AdaptiveCardInvoke
    {
        [JsonProperty("action")]
        public AdaptiveCardAction Action { get; set; }
    }
}
