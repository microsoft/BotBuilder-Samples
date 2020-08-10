using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ActionsBlock : ISlackBlock
    {
        public string type { get; } = "actions";
        public string block_id { get; set; }
        [JsonConverter(typeof(BlockElementArrayConverter))]
        public IBlockElement[] elements { get; set; }
        public JObject properties { get; set; }
    }
}
