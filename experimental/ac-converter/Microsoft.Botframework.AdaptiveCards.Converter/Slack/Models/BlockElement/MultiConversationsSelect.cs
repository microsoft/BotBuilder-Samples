using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class MultiConversationsSelect : IBlockElement
    {
        public string type { get; set; } = "multi_conversations_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public string[] initial_conversations { get; set; }
        public bool default_to_current_conversation { get; set; }
        public ConfirmObject confirm { get; set; }
        public int max_selected_items { get; set; } = 1;
        public FilterObject filter { get; set; }
    }
}
