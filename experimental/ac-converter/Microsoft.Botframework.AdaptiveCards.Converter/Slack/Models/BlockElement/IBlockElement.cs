using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public interface IBlockElement
    {
        string type { get;}
        JObject properties { get; set; }
    }
}
