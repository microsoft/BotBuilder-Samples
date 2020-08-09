using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class MultiChannelsSelect : IBlockElement
    {
        public string type { get; set; } = "multi_channels_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public string[] initial_channels { get; set; }
        public ConfirmObject confirm { get; set; }
        public int max_selected_items { get; set; } = 1;
    }
}
