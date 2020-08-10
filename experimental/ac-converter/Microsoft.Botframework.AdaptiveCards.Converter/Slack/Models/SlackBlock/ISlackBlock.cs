using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public interface ISlackBlock
    {
        string type { get; }
        string block_id { get; set; }
        JObject Properties { get; set; }
    }
}
