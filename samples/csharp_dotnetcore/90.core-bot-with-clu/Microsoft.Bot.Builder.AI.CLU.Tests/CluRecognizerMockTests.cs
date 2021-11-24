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
    public class CluRecognizerMockTests
    {
        private readonly CluApplication _cluApp;
        private readonly String fakeProjectName = "fakeProjectName";
        private readonly String fakeDeploymentName = "fakeDeploymentName";

        public CluRecognizerMockTests()
        {
            _cluApp = new CluApplication(fakeProjectName, fakeDeploymentName, Guid.NewGuid().ToString(), "https://somecluendpoint");
        }

        [Fact]
        public async Task OptionsAreUsed()
        {
            var testUtterance = "book a flight from Cairo to London on the first of December 2021";
            var expectedOptions = new CluOptions(_cluApp)
            {
                Verbose = false,
                IsLoggingEnabled = false,
                Language = "es",
                DirectTarget = "TrainBooking"
            };

            
            var mockClient = new MockConversationAnalysisClient();

            var sut = new CluRecognizer(expectedOptions, mockClient);
            try
            {
                var response = await sut.RecognizeAsync(TestUtilities.BuildTurnContextForUtterance(testUtterance), CancellationToken.None);
            }
            catch(Exception)
            {
                // expecting Recognizer to throw exception since mock client returns invalid response
            }

            var analyzeConversationOptions = mockClient.analyzeConversationOptions;

            Assert.Equal(expectedOptions.Verbose, analyzeConversationOptions.Verbose);
            Assert.Equal(expectedOptions.IsLoggingEnabled, analyzeConversationOptions.IsLoggingEnabled);
            Assert.Equal(expectedOptions.Language, analyzeConversationOptions.Language);
            Assert.Equal(expectedOptions.DirectTarget, analyzeConversationOptions.DirectTarget);
        }
    }
}
