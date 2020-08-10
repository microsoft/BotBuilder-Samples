using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class FileBlock : ISlackBlock
    {
        public string type { get; } = "file";
        public string external_id { get; set; }
        public string source { get; set; }
        public string block_id { get; set; }
        public JObject properties { get; set; }
    }
}
