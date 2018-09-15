// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Facebook_Events_Bot.FacebookModel
{
    /// <summary>
    /// A Facebook optin event payload definition.
    /// </summary>
    /// <remarks>See <see cref="https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/"> messaging optin Facebook documentation</see> for more information on messaging_optin.</remarks>
    public class FacebookOptin
    {
        /// <summary>
        /// Gets or sets the optin data ref.
        /// </summary>
        [JsonProperty("ref")]
        public string Ref { get; set; }

        /// <summary>
        /// Gets or sets the optin user ref.
        /// </summary>
        [JsonProperty("user_ref")]
        public string UserRef { get; set; }
    }
}
