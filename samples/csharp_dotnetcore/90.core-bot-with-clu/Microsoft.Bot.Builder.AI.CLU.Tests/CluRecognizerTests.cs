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
    public class CluRecognizerTests
    {
        private readonly CluApplication _cluApp;
        private readonly EmptyCluResponseClientHandler _mockHttpClientHandler;
        private readonly String fakeProjectName = "fakeProjectName";
        private readonly String fakeDeploymentName = "fakeDeploymentName";

        public CluRecognizerTests()
        {
            _cluApp = new CluApplication(fakeProjectName, fakeDeploymentName, Guid.NewGuid().ToString(), "https://somecluendpoint");
            _mockHttpClientHandler = new EmptyCluResponseClientHandler();
        }

        [Theory]
        [InlineData(true, true, "en", "2021-07-15-preview", "production", "hi")]
        [InlineData(false, false, "de", "2021-07-15-preview", "production", "hi")]
        [InlineData(null, null, null, "2021-05-01-preview", "staging", "hello")]
        public async Task CluOptionsAndUtteranceAreUsedInTheRequest(bool? verbose, bool? isLoggingEnabled, string language, string apiVersion, string slot, string query)
        {
            // Arrange
            var expectedOptions = new CluOptions(_cluApp)
            {
                Verbose = verbose,
                IsLoggingEnabled = isLoggingEnabled,
                Language = language,
                ApiVersion = apiVersion,
            };

            var sut = new CluRecognizer(expectedOptions, _mockHttpClientHandler);

            // Act
            await sut.RecognizeAsync(BuildTurnContextForUtterance(query), CancellationToken.None);

            // Assert
            await AssertCluRequest(_mockHttpClientHandler.RequestMessage, expectedOptions, query);
        }

        [Theory]
        [InlineData("ContosoApp", "model1", "https://Contoso.cognitiveservices.azure.com", "fb52afe0ee8a42c9b20351ea93876ed8")]
        [InlineData("FlightBooking", "production", "https://a.cognitiveservices.azure.com", "331e2f4c2ef541c7beeeb62e19889c69")]
        [InlineData("TestApp", "model2", "https://Test.cognitiveservices.azure.com", "0d9decc0c59a48e39236b36fa5d3cc27")]
        public async Task CluApplicationUsedInTheRequest(string projectName, string deploymentName, string apiHostName, string apiKey)
        {
            // Note: These are not real applications and the keys have been randomly generated

            // Arrange
            var application = new CluApplication(projectName, deploymentName, apiKey, apiHostName);
            var options = new CluOptions(application);

            // Act
            var sut = new CluRecognizer(options, _mockHttpClientHandler);
            await sut.RecognizeAsync(BuildTurnContextForUtterance("hi"), CancellationToken.None);

            // Assert
            AssertCluApplication(_mockHttpClientHandler.RequestMessage, application);
        }

        private static void AssertCluApplication(HttpRequestMessage httpRequestForClu, CluApplication application)
        {
            var queryStringParameters = HttpUtility.ParseQueryString(httpRequestForClu.RequestUri.Query);
            var headers = httpRequestForClu.Headers.GetValues("Ocp-Apim-Subscription-Key");

            ////Assert Request URI Parameters
            Assert.Equal(application.ProjectName, queryStringParameters["projectName"].ToString());
            Assert.Equal(application.DeploymentName, queryStringParameters["deploymentName"].ToString());
            Assert.Equal(application.Endpoint.ToLower(), "https://" + httpRequestForClu.RequestUri.Host);
            Assert.Equal(application.EndpointKey, headers.FirstOrDefault());
        }

        private static async Task AssertCluRequest(HttpRequestMessage httpRequestForClu, CluOptions expectedOptions, string query)
        {
            var queryStringParameters = HttpUtility.ParseQueryString(httpRequestForClu.RequestUri.Query);
            var requestBodyParameters = JsonConvert.DeserializeObject<JObject>(await httpRequestForClu.Content.ReadAsStringAsync());

            //Assert Request URI Parameters
            Assert.Equal(expectedOptions.ApiVersion, queryStringParameters["api-version"].ToString());

            //Assert Request Body Parameters
            Assert.Equal(query, requestBodyParameters["query"].ToString());
            Assert.Equal(expectedOptions.Language, requestBodyParameters["language"]?.ToString());
            Assert.Equal(expectedOptions.Verbose?.ToString(), requestBodyParameters["verbose"]?.ToString());
            Assert.Equal(expectedOptions.IsLoggingEnabled?.ToString(), requestBodyParameters["isLoggingEnabled"]?.ToString());
        }

        private static TurnContext BuildTurnContextForUtterance(string utterance)
        {
            var testAdapter = new TestAdapter();
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                Text = utterance,
                Conversation = new ConversationAccount(),
                Recipient = new ChannelAccount(),
                From = new ChannelAccount(),
            };
            return new TurnContext(testAdapter, activity);
        }
    }
}
