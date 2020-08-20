// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration, OrchestratorRecognizer dispatcher)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            // If includeApiResults is set to true, the full response from the LUIS api (LuisResult)
            // will be made available in the properties collection of the RecognizerResult
            LuisHomeAutomationRecognizer = CreateLuisRecognizer(configuration, "LuisHomeAutomationAppId");
            LuisWeatherRecognizer = CreateLuisRecognizer(configuration, "LuisWeatherAppId");

            Dispatch = dispatcher;

            SampleQnA = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAEndpointKey"],
                Host = configuration["QnAEndpointHostName"]
            });
        }

        public OrchestratorRecognizer Dispatch { get; private set; }
        
        public QnAMaker SampleQnA { get; private set; }
        
        public LuisRecognizer LuisHomeAutomationRecognizer { get; private set; }

        public LuisRecognizer LuisWeatherRecognizer { get; private set; }

        private LuisRecognizer CreateLuisRecognizer(IConfiguration configuration, string appIdKey)
        {
            var luisApplication = new LuisApplication(
                configuration[appIdKey],
                configuration["LuisAPIKey"],
                configuration["LuisAPIHostName"]);

            // Set the recognizer options depending on which endpoint version you want to use.
            // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            return new LuisRecognizer(recognizerOptions);
        }
    }
}
