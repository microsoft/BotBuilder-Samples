using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class UsersSelect : IBlockElement
    {
        public string type { get; } = "users_select";
        public TextObject placeholder { get; set; }
        public string action_id { get; set; }
        public string initial_users { get; set; }
        public ConfirmObject confirm { get; set; }
        public JObject Properties { get; set; }
    }
}
