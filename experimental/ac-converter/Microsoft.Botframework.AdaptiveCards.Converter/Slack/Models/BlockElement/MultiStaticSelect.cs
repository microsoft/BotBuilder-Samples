using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class MultiStaticSelect : IBlockElement
    {
        public string type { get; } = "multi_static_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public OptionObject[] options { get; set; }
        public OptionGroupObject[] option_groups { get; set; }
        public object[] initial_options { get; set; }
        public ConfirmObject confirm { get; set; }
        public int max_selected_items { get; set; } = 1;
        public JObject Properties { get; set; }
    }
}
