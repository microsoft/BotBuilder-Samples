using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class DividerBlock : ISlackBlock
    {
        public string type { get; } = "divider";
        public string block_id { get; set; }
        public JObject properties { get; set; }
    }
}
