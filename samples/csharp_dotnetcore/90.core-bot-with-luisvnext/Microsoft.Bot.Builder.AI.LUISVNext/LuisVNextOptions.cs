// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.AI.LuisVNext
{
    public class LuisVNextOptions
    {
        /// <summary>
        /// An instance of the <see cref="LuisApplication"/> class containing connection details for you LuisVNext application.
        /// </summary>
        [JsonProperty("luisApplication")]
        internal LuisApplication luisApplication;

        /// <summary>
        /// Creates an instance of  <see cref="LuisVNextOptions"/> containing the Luis Application as well as optional configurations.
        /// </summary>
        public LuisVNextOptions(LuisApplication app)
        {
            luisApplication = app;
        }

        /// <summary>
        /// If set to true, the service will return a more verbose response.
        /// </summary>
        [JsonProperty("verbose")]
        public bool? Verbose;

        /// <summary>
        /// If true, the query will be kept by the service for customers to further review, to improve the model quality.
        /// </summary>
        [JsonProperty("isLoggingEnabled")]
        public bool? IsLoggingEnabled;

        /// <summary>
        /// The language to be used with this recognizer.
        /// </summary>
        [JsonProperty("language")]
        public string Language;

        /// <summary>
        /// The version of the api to use.
        /// </summary>
        [JsonProperty("apiVersion")]
        public string ApiVersion = "2021-07-15-preview";

        /// <summary>
        /// The deployment slot for the LuisVNext application
        /// </summary>
        [JsonProperty("slot")]
        public string Slot = LuisSlot.Production;


        internal string ApplicationId => luisApplication.ApplicationId;
        internal string EndpointKey => luisApplication.EndpointKey;
        internal string Endpoint => luisApplication.Endpoint;

    }
}
