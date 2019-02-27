// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        private IConfiguration _configuration;

        public BotServices(IConfiguration configuration)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            HomeAutomation = ReadLuisRecognizer(configuration, "Home-Automation");
            Weather = ReadLuisRecognizer(configuration, "Weather");
            Dispatch = ReadLuisRecognizer(configuration, "nlp-with-dispatchDispatch");
            SampleQnA = ReadQnAMaker(configuration, "sample-qna");
        }

        public LuisRecognizer HomeAutomation { get; private set; }
        public LuisRecognizer Weather { get; private set; }
        public LuisRecognizer Dispatch { get; private set; }
        public QnAMaker SampleQnA { get; private set; }

        private LuisRecognizer ReadLuisRecognizer(IConfiguration configuration, string name)
        {
            try
            {
                var services = configuration.GetSection("BotServices");
                var luisService = new LuisService
                {
                    AppId = services.GetValue<string>("Luis-" + name + "-AppId"),
                    AuthoringKey = services.GetValue<string>("Luis-" + name + "-Authoringkey"),
                    Region = services.GetValue<string>("Luis-" + name + "-Region")
                };
                return new LuisRecognizer(new LuisApplication(
                    luisService.AppId,
                    luisService.AuthoringKey,
                    luisService.GetEndpoint()));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private QnAMaker ReadQnAMaker(IConfiguration configuration, string name)
        {
            try
            {
                var services = configuration.GetSection("BotServices");
                return new QnAMaker(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = services.GetValue<string>("QnA-" + name + "-kbId"),
                    EndpointKey = services.GetValue<string>("QnA-" + name + "-endpointKey"),
                    Host = services.GetValue<string>("QnA-" + name + "-hostname")
                });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
