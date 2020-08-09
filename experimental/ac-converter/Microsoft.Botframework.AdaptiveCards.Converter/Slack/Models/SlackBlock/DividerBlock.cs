using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class DividerBlock : ISlackBlock
    {
        public string type { get; set; } = "divider";
        public string block_id { get; set; }
    }
}
