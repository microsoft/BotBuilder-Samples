// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.FacebookModel
{
    /// <summary>
    /// Definition for Facebook PostBack payload. Present on calls from
    /// messaging_postback webhook event.
    /// </summary>
    /// <remarks>See <see cref="https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/"> Facebook messaging_postback</see> webhook event documentation.</remarks>
    public class FacebookPostback
    {
        /// <summary>
        /// Gets or sets payload of the PostBack. Could be an object depending on the object sent.
        /// </summary>
        [JsonProperty("payload")]
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the title of the postback.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
