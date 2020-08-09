using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class OverflowMenu : IBlockElement
    {
        public string type { get; set; } = "overflow";
        public string action_id { get; set; }
        public OptionObject[] options { get; set; }
        public ConfirmObject confirm { get; set; }
    }
}
