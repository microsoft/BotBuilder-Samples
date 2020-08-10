using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class HeaderBlock : ISlackBlock
    {
        public string type { get; } = "header";
        public TextObject text { get; set; }
        public string block_id { get; set; }
        public JObject properties { get; set; }
    }
}
