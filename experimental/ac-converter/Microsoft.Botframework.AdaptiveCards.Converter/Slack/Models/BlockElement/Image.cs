using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class Image : IBlockElement
    {
        public string type { get; set; } = "image";
        public string image_url { get; set; }
        public string alt_text { get; set; }
    }
}
