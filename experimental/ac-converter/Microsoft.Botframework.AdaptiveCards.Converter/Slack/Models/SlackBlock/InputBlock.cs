using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class InputBlock : ISlackBlock
    {
        public string type { get; set; } = "input";
        public string block_id { get; set; }
        public TextObject label { get; set; }
        public object elements { get; set; }
        public TextObject hint { get; set; }
        public bool? optional { get; set; }
    }
}
