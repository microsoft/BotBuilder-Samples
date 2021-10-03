// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Bot.Builder.AI.CLU
{
    /// <summary>
    /// Data describing a CLU application.
    /// </summary>
    public class CluApplication
    {
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="CluApplication"/> class for mocking.
        /// </summary>
        public CluApplication()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CluApplication"/> class.
        /// </summary>
        /// <param name="projectName">CLU project name.</param>
        /// <param name="deploymentName">CLU model deployment name.</param>
        /// <param name="endpointKey">CLU subscription or endpoint key.</param>
        /// <param name="endpoint">CLU endpoint to use like https://mytextanalyticsresource.cognitive.azure.com.</param>
        public CluApplication(string projectName, string deploymentName, string endpointKey, string endpoint)
            : this((projectName,deploymentName, endpointKey, endpoint))
        {
        }

        private CluApplication(ValueTuple<string, string, string, string> props)
        {
            var (projectName, deploymentName, endpointKey, endpoint) = props;

            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentNullException($"projectName value is Null or whitespace. Please use a valid projectName.");
            }

            if (string.IsNullOrWhiteSpace(deploymentName))
            {
                throw new ArgumentNullException($"deploymentName value is Null or whitespace. Please use a valid deploymentName.");
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
                throw new ArgumentException($"\"{endpointKey}\" is not a valid CLU subscription key.");
            }


            if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
            {
                throw new ArgumentException($"\"{endpoint}\" is not a valid CLU endpoint.");
            }

            ProjectName = projectName;
            DeploymentName = deploymentName;
            EndpointKey = endpointKey;
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets or sets the CLU project name.
        /// </summary>
        /// <value>
        /// CLU project name.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets CLU model deployment name.
        /// </summary>
        /// <value>
        /// CLU model deployment name.
        /// </value>
        public string DeploymentName { get; set; }

        /// <summary>
        /// Gets or sets CLU subscription or endpoint key.
        /// </summary>
        /// <value>
        /// CLU subscription or endpoint key.
        /// </value>
        public string EndpointKey { get; set; }

        /// <summary>
        /// Gets or sets CLU endpoint like https://mytextanalyticsresource.cognitive.azure.com.
        /// </summary>
        /// <value>
        /// CLU endpoint where application is hosted.
        /// </value>
        public string Endpoint { get; set; }

    }
}
