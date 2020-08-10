using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ContextBlock : ISlackBlock
    {
        public string type { get; } = "context";
        public string block_id { get; set; }
        public object[] elements { get; set; }
        public JObject properties { get; set; }
    }
}
