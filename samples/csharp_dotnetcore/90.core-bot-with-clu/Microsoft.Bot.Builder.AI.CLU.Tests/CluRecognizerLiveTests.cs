// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Azure;
using Azure.AI.Language.Conversations;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;


namespace Microsoft.Bot.Builder.AI.CLU.Tests
{
    public class CluRecognizerLiveTests
    {
        private readonly CluApplication _cluApp;
        private readonly CluApplication _cluOrchApp;
        private CluRecognizer _cluRecognizer;
        JObject configs;
        public CluRecognizerLiveTests()
        {
            configs = ReadTestResourceConfigs();
            _cluApp = ExtractConversationApp("Conversations");
            _cluOrchApp = ExtractConversationApp("Orchestration");
        }

        [Fact]
        public async Task ConversationProjectRecognize()
        {
            var testUtterance = "book a flight from Cairo to London on the first of December 2021";
            var options = new CluOptions(_cluApp);

            var sut = new CluRecognizer(options);
            var response = await sut.RecognizeAsync(TestUtilities.BuildTurnContextForUtterance(testUtterance), CancellationToken.None);

            Assert.Equal(testUtterance, response.Text);
            Assert.True(response.Intents["BookFlight"].Score > 0.8);
            Assert.Equal("BookFlight", response.Properties["topIntent"]);
            Assert.Equal("conversation", response.Properties["projectKind"]);
            TestUtilities.AssertCluEntities(response.Entities, "Cairo", "London", "first of December 2021");
        }

        [Fact]
        public async Task OrchestrationProjectCluRecognize()
        {
            var testUtterance = configs["OrchestrationCluQuery"].Value<string>();
            var options = new CluOptions(_cluOrchApp);

            var sut = new CluRecognizer(options);
            var response = await sut.RecognizeAsync(TestUtilities.BuildTurnContextForUtterance(testUtterance), CancellationToken.None);

            Assert.Equal(testUtterance, response.Text);
            Assert.Equal("workflow", response.Properties["projectKind"]);
            Assert.True(response.Intents.Count > 0);

            Assert.True(!String.IsNullOrEmpty(response.Properties["topIntent"].ToString()));

            var cluEntityList = JsonConvert.DeserializeObject<CluEntity[]>(response.Entities["entities"].ToString());
            Assert.True(cluEntityList.Count() > 0);
            Assert.True(!String.IsNullOrEmpty(cluEntityList[0].category));
        }

        [Fact]
        public async Task OrchestrationProjectQARecognize()
        {
            var testUtterance = configs["OrchestrationQAQuery"].Value<string>();
            var options = new CluOptions(_cluOrchApp);

            var sut = new CluRecognizer(options);
            var response = await sut.RecognizeAsync(TestUtilities.BuildTurnContextForUtterance(testUtterance), CancellationToken.None);

            Assert.Equal(testUtterance, response.Text);
            Assert.Equal("workflow", response.Properties["projectKind"]);
            Assert.True(response.Intents.Count > 0);

            Assert.True(response.Intents.ContainsKey(CluRecognizer.QuestionAnsweringMatchIntent));
            Assert.True(response.Intents[CluRecognizer.QuestionAnsweringMatchIntent].Score > 0);

            var answerArr = (JArray)response.Entities["answer"];
            Assert.True(answerArr.Count == 1);
            Assert.True(!String.IsNullOrEmpty(answerArr.FirstOrDefault().Value<string>()));

            var allAnswersArray = (IReadOnlyList<KnowledgeBaseAnswer>)response.Properties["answers"];
            var firstAnswer = allAnswersArray.FirstOrDefault();

            Assert.True(allAnswersArray.Count >= 1);
            Assert.True(!String.IsNullOrEmpty(firstAnswer.Answer));
            Assert.True(firstAnswer.ConfidenceScore >= 0);
        }
        // todo qa and luis tests

        [Fact]
        public async Task OrchestrationProjectLUISRecognize()
        {
            var testUtterance = configs["OrchestrationLuisQuery"].Value<string>();
            var options = new CluOptions(_cluOrchApp);

            var sut = new CluRecognizer(options);
            var response = await sut.RecognizeAsync(TestUtilities.BuildTurnContextForUtterance(testUtterance), CancellationToken.None);

            Assert.Equal(testUtterance, response.Text);
            Assert.Equal("workflow", response.Properties["projectKind"]);
            Assert.True(response.Intents.Count > 0);


            // There doesn't really seem any other simple way to generically assert for presence
            // of LUIS entities due to the complexity of the object. The Object's structure and property
            // names depends on the actual project itself.

            var entitiesObject = response.Entities;
            Assert.Contains("text", entitiesObject.ToString());
            Assert.Contains("startIndex", entitiesObject.ToString());
            Assert.Contains("endIndex", entitiesObject.ToString());
        }

        private CluApplication ExtractConversationApp(string projectType)
        {
            string projectName, deploymentName, endpoint, key;
            CluApplication testApplication;

            try
            {
                projectName = configs[projectType + "ProjectName"].Value<string>();
                deploymentName = configs[projectType + "DeploymentName"].Value<string>();
                endpoint = "https://" + configs[projectType + "APIHostName"].Value<string>();
                key = configs[projectType + "APIKey"].Value<string>();

                testApplication = new CluApplication(projectName, deploymentName, key, endpoint);
                return testApplication;

            }
            catch (Exception e)
            {
                throw new Exception("Problem occured while parsing livetestresource.json. Please ensure that all necessary parameters are in place and are valid.", e);
            }
        }
        private JObject ReadTestResourceConfigs()
        {
            var fileContent = File.ReadAllText("livetestresource.json");
            var configObject = JObject.Parse(fileContent);
            return configObject;
        }
    }
}
