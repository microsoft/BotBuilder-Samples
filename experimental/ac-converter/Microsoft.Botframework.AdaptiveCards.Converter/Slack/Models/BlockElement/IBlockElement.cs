using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public interface IBlockElement
    {
        string type { get; set; }
    }
}
