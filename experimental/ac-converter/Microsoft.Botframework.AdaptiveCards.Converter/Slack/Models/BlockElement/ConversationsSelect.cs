using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ConversationsSelect : IBlockElement
    {
        public string type { get; set; } = "conversations_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public string initial_conversation { get; set; }
        public bool default_to_current_conversation { get; set; }
        public ConfirmObject confirm { get; set; }
        public bool response_url_enabled { get; set; }
        public FilterObject filter { get; set; }
    }
}
