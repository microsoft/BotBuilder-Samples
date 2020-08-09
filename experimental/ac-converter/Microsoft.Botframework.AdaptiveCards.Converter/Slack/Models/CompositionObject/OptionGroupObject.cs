using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class OptionGroupObject
    {
        public TextObject label { get; set; }
        public OptionObject[] options { get; set; }
    }
}
