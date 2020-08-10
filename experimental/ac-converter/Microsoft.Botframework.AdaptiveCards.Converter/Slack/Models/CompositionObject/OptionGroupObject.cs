using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class OptionGroupObject
    {
        public TextObject label { get; set; }
        public OptionObject[] options { get; set; }
        public JObject Properties { get; set; }
    }
}
