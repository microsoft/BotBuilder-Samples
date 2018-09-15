// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

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
