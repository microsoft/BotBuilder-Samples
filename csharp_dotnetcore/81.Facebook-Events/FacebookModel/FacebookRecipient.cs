using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Events_Bot.FacebookModel
{

    /// <summary>
    /// Defines a Facebook recipient.
    /// </summary>
    public class FacebookRecipient
    {
        /// <summary>
        /// The Facebook Id of the recipient.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
