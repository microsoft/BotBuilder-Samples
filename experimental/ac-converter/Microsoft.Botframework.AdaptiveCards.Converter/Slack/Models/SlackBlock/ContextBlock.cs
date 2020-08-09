using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ContextBlock : ISlackBlock
    {
        public string type { get; set; } = "context";
        public string block_id { get; set; }
        public object[] elements { get; set; }
    }
}
