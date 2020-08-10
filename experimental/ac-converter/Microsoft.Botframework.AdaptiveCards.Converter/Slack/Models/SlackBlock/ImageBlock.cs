using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class ImageBlock : ISlackBlock
    {
        public string type { get; } = "image";
        public string block_id { get; set; }
        public string image_url { get; set; }
        public string alt_text { get; set; } = "alt_text";
        public string title { get; set; }
        public JObject Properties { get; set; }
    }
}
