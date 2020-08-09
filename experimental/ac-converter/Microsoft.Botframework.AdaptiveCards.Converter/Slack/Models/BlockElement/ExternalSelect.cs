using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ExternalSelect : IBlockElement
    {
        public string type { get; set; } = "external_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public object initial_option { get; set; }
        public int min_query_length { get; set; } = 3;
        public ConfirmObject confirm { get; set; }
    }
}
