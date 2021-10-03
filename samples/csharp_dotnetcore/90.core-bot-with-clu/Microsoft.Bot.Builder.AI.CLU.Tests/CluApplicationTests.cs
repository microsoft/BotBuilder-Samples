// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Bot.Builder.AI.CLU.Tests
{
    public class CluApplicationTests
    {
        //These values are valid and would pass CLU Aplication validation checks
        private const string validProjectName = "FlightBooking";
        private const string validDeploymentName = "ModelVersion1";
        private const string validEndpointKey = "331e2f4c2ef541c7beeeb62e19889c69";
        private const string validEndpoint = "https://contoso.cognitiveservices.azure.com";
        private const string emptyString = "";


        [Theory]
        [InlineData(validProjectName, validDeploymentName, "0000", validEndpoint)]
        [InlineData(validProjectName, validDeploymentName, "331e2f4c2ef541c7beeeb62e19889c6", validEndpoint)]
        [InlineData(validProjectName, validDeploymentName, validEndpointKey, "contoso.cognitiveservices.azure.com")]
        [InlineData(validProjectName, validDeploymentName, validEndpointKey, "contoso")]
        [InlineData(validProjectName, validDeploymentName, validEndpointKey, "https://")]
        public async Task CluApplicationArgumentException(string projectName, string deploymentName, string endpointKey, string endpoint)
        {
            //invalid App Name
            Assert.Throws<ArgumentException>(() => new CluApplication(projectName, deploymentName, endpointKey, endpoint));
        }

        [Theory]
        [InlineData(null, validDeploymentName, validEndpointKey, validEndpoint)]
        [InlineData(emptyString, validDeploymentName, validEndpointKey, validEndpoint)]
        [InlineData(validProjectName, null, validEndpointKey, validEndpoint)]
        [InlineData(validProjectName, emptyString, validEndpointKey, validEndpoint)]
        [InlineData(validProjectName, validDeploymentName, null, validEndpoint)]
        [InlineData(validProjectName, validDeploymentName, emptyString, validEndpoint)]
        [InlineData(validProjectName, validDeploymentName, validEndpointKey, null)]
        [InlineData(validProjectName, validDeploymentName, validEndpointKey, emptyString)]
        public async Task CluApplicationArgumentNullException(string projectName, string deploymentName, string endpointKey, string endpoint)
        {
            //invalid App Name
            Assert.Throws<ArgumentNullException>(() => new CluApplication(projectName, deploymentName, endpointKey, endpoint));
        }
    }
}
