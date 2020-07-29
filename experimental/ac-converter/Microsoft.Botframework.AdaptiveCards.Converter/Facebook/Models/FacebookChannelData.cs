using Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Models.Outgoing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Models
{
    /// <summary>
    /// Acceptable channelData of current Facebook service.
    /// </summary>
    public class FacebookChannelData
    {
        public FacebookChannelData()
        {
        }

        public static FacebookChannelData CreateFromObject(object obj)
        {
            if (obj is JObject)
            {
                return ((JObject)obj).ToObject<FacebookChannelData>();

            }

            return JsonConvert.DeserializeObject<FacebookChannelData>(JsonConvert.SerializeObject(obj));
        }

        [JsonProperty("notification_type")]
        public string NotificationType { get; set; }

        /// <summary>
        /// Defaults to RESPONSE but bots can specify other values such as UPDATE or MESSAGE_TAG
        /// https://developers.facebook.com/docs/messenger-platform/send-messages#messaging_types
        /// </summary>
        [JsonProperty("messaging_type")]
        public string MessagingType { get; set; }


        /// <summary>
        /// https://developers.facebook.com/docs/messenger-platform/send-messages/message-tags
        /// required when messaging_type==MESSAGE_TAG
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Facebook format attachment
        /// </summary>
        [JsonProperty("attachment")]
        public object Attachment { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("quick_replies")]
        public object QuickReplies { get; set; }

        [JsonProperty("sender_action")]
        public string SenderAction { get; set; }

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public JObject Properties { get; set; }
    }
}
