using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class DatePicker : IBlockElement
    {
        public string type { get; set; } = "datepicker";
        public string action_id { get; set; }
        public TextObject placeholder { get; set; }
        public string initial_date { get; set; }
        public ConfirmObject confirm { get; set; }
    }
}
