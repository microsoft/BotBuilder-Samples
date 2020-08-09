using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class Button : IBlockElement
    {
        public string type { get; set; } = "button";
        public TextObject text { get; set; }
        public string action_id { get; set; }
        public string url { get; set; }
        public string value { get; set; }
        public string style { get; set; }
        public ConfirmObject confirm { get; set; }
    }
}
