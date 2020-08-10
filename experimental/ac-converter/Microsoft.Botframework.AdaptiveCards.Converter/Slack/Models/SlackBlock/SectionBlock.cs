using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class SectionBlock : ISlackBlock
    {
        public string type { get; } = "section";
        public string block_id { get; set; }
        public TextObject text { get; set; }
        public TextObject[] fields { get; set; }
        [JsonConverter(typeof(BlockElementConverter))]
        public IBlockElement accessory { get; set; }
        public JObject Properties { get; set; }
    }
}
