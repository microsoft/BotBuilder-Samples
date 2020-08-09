using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public interface ISlackBlock
    {
        string type { get; set; }
        string block_id { get; set; }
    }
}
