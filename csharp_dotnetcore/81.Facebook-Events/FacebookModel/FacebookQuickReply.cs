using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook_Events_Bot.FacebookModel
{

    /// <summary>
    /// A Facebook quick reply.
    /// </summary>
    /// <remarks>See <see cref="https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/"> Quick Replies Facebook Documentation</see> for more information on quick replies.</remarks>
    public class FacebookQuickReply
    {
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}
