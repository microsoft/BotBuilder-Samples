// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.AI.Language.Conversations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Bot.Builder.AI.CLU
{
    public class CluOptions
    {
        /// <summary>
        /// An instance of the <see cref="CluApplication"/> class containing connection details for your CLU application.
        /// </summary>
        [JsonProperty("cluApplication")]
        internal CluApplication cluApplication;

        /// <summary>
        /// Creates an instance of  <see cref="CluOptions"/> containing the CLU Application as well as optional configurations.
        /// </summary>
        public CluOptions(CluApplication app)
        {
            cluApplication = app;
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
        public ConversationAnalysisClientOptions.ServiceVersion ApiVersion = ConversationAnalysisClientOptions.ServiceVersion.V2021_11_01_Preview;

        /// <summary>
        /// The name of the target project this request is sending to directly.
        /// </summary>
        [JsonProperty("DirectTarget")]
        public string DirectTarget { get; set; }

        /// <summary>
        /// A dictionary representing the input for each target project.
        /// </summary>
        [JsonProperty("Parameters")]
        public IDictionary<string, AnalysisParameters> Parameters { get; }

        internal string ProjectName => cluApplication.ProjectName;
        internal string DeploymentName => cluApplication.DeploymentName;
        internal string EndpointKey => cluApplication.EndpointKey;
        internal string Endpoint => cluApplication.Endpoint;

    }
}
