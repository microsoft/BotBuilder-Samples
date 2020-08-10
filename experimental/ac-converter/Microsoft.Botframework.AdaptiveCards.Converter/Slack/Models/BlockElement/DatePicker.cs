using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class DatePicker : IBlockElement
    {
        public string type { get; } = "datepicker";
        public string action_id { get; set; }
        public TextObject placeholder { get; set; }
        public string initial_date { get; set; }
        public ConfirmObject confirm { get; set; }
        public JObject Properties { get; set; }
    }
}
