using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class MultiExternalSelect : IBlockElement
    {
        public string type { get; } = "multi_external_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public int min_query_length { get; set; } = 3;
        public object[] initial_options { get; set; }
        public ConfirmObject confirm { get; set; }
        public int max_selected_items { get; set; } = 1;
        public JObject properties { get; set; }
    }
}
