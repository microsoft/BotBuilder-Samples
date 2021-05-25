// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.FacebookModel
{
    /// <summary>
    /// A Facebook message payload.
    /// </summary>
    public class FacebookMessage
    {
        /// <summary>
        /// Gets or sets the message Id from Facebook.
        /// </summary>
        [JsonProperty("mid")]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets whether the message is an echo message.
        /// See <see cref="https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/message-echoes/">Echo Message</see>
        /// in the Facebook Developer Documentation.
        /// </summary>
        [JsonProperty("is_echo")]
        public bool IsEcho { get; set; }

        /// <summary>
        /// Gets or sets the quick reply.
        /// </summary>
        [JsonProperty("quick_reply")]
        public FacebookQuickReply QuickReply { get; set; }
    }
}
