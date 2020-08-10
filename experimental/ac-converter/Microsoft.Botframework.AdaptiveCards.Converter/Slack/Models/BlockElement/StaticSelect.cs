using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class StaticSelect : IBlockElement
    {
        public string type { get; } = "static_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public OptionObject[] options { get; set; }
        public OptionGroupObject[] option_groups { get; set; }
        public object initial_option { get; set; }
        public ConfirmObject confirm { get; set; }
        public JObject properties { get; set; }
    }
}
