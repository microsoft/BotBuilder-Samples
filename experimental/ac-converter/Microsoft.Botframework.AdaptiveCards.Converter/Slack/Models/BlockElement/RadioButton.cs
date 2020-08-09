using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class RadioButton : IBlockElement
    {
        public string type { get; set; } = "radio_buttons";
        public string action_id { get; set; }
        public OptionObject[] options { get; set; }
        public OptionObject initial_option { get; set; }
        public ConfirmObject confirm { get; set; }
    }
}
