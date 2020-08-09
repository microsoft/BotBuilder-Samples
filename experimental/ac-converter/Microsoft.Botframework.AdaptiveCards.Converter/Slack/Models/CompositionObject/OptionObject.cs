using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class OptionObject
    {
        public TextObject text { get; set; }
        public string value { get; set; }
        public TextObject description { get; set; }
        public string url { get; set; }
    }
}
