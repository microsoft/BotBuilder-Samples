// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Bot.Builder.AI.LuisVNext
{
    /// <summary>
    /// Data describing a LUIS application.
    /// </summary>
    public class LuisApplication
    {
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="LuisApplication"/> class for mocking.
        /// </summary>
        public LuisApplication()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LuisApplication"/> class.
        /// </summary>
        /// <param name="applicationId">LUIS application ID.</param>
        /// <param name="endpointKey">LUIS subscription or endpoint key.</param>
        /// <param name="endpoint">LUIS endpoint to use like https://westus.api.cognitive.microsoft.com.</param>
        public LuisApplication(string applicationId, string endpointKey, string endpoint)
            : this((applicationId, endpointKey, endpoint))
        {
        }

        private LuisApplication(ValueTuple<string, string, string> props)
        {
            var (applicationId, endpointKey, endpoint) = props;

            if (string.IsNullOrWhiteSpace(applicationId))
            {
                throw new ArgumentNullException($"applicationId value is Null or whitespace. Please use a valid applicationId.");
            }

            if (string.IsNullOrWhiteSpace(endpointKey))
            {
                throw new ArgumentNullException($"endpointKey value is Null or whitespace. Please use a valid endpointKey.");
            }

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException($"Endpoint value is Null or whitespace. Please use a valid endpoint.");
            }

            if (!Guid.TryParse(endpointKey, out var _))
            {
                throw new ArgumentException($"\"{endpointKey}\" is not a valid LUIS subscription key.");
            }


            if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
            {
                throw new ArgumentException($"\"{endpoint}\" is not a valid LUIS endpoint.");
            }

            ApplicationId = applicationId;
            EndpointKey = endpointKey;
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets or sets LuisVNext application ID.
        /// </summary>
        /// <value>
        /// LUIS application ID.
        /// </value>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets LuisVNext subscription or endpoint key.
        /// </summary>
        /// <value>
        /// LUIS subscription or endpoint key.
        /// </value>
        public string EndpointKey { get; set; }

        /// <summary>
        /// Gets or sets LuisVNext endpoint like https://westus.api.cognitive.microsoft.com.
        /// </summary>
        /// <value>
        /// LUIS endpoint where application is hosted.
        /// </value>
        public string Endpoint { get; set; }

    }
}
