using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class PlainTextInput : IBlockElement
    {
        public string type { get; } = "plain_text_input";
        public string action_id { get; set; }
        public TextObject placeholder { get; set; }
        public string initial_value { get; set; }
        public bool? multiline { get; set; }
        public int min_length { get; set; }
        public int max_length { get; set; }
        public JObject Properties { get; set; }
    }
}
