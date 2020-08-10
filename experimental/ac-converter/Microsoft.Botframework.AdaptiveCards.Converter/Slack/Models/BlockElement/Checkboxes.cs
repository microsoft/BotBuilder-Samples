using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class Checkboxes : IBlockElement
    {
        public string type { get; } = "checkboxes";
        public string action_id { get; set; }
        public OptionObject[] options { get; set; }
        public OptionObject[] initial_options { get; set; }
        public ConfirmObject confirm { get; set; }
        public JObject Properties { get; set; }
    }
}
