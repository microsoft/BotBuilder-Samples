// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.FacebookModel
{
    /// <summary>
    /// Defines a Facebook sender.
    /// </summary>
    public class FacebookSender
    {
        /// <summary>
        /// The Facebook Id of the sender.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
