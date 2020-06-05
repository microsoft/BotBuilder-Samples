// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Solutions;

namespace LivePersonProxyAssistant.Services
{
    public class BotServices
    {
        public BotServices()
        {
        }

        public BotServices(BotSettings settings, IBotTelemetryClient client)
        {
            foreach (var pair in settings.CognitiveModels)
            {
                var set = new CognitiveModelSet();
                var language = pair.Key;
                var config = pair.Value;

                var telemetryClient = client;

                LuisRecognizerOptionsV3 luisOptions;

                if (config.DispatchModel != null)
                {
                    var dispatchApp = new LuisApplication(config.DispatchModel.AppId, config.DispatchModel.SubscriptionKey, config.DispatchModel.GetEndpoint());
                    luisOptions = new LuisRecognizerOptionsV3(dispatchApp)
                    {
                        TelemetryClient = telemetryClient,
                        LogPersonalInformation = true,
                    };
                    set.DispatchService = new LuisRecognizer(luisOptions);
                }

                if (config.LanguageModels != null)
                {
                    foreach (var model in config.LanguageModels)
                    {
                        var luisApp = new LuisApplication(model.AppId, model.SubscriptionKey, model.GetEndpoint());
                        luisOptions = new LuisRecognizerOptionsV3(luisApp)
                        {
                            TelemetryClient = telemetryClient,
                            LogPersonalInformation = true,
                        };
                        set.LuisServices.Add(model.Id, new LuisRecognizer(luisOptions));
                    }
                }

                foreach (var kb in config.Knowledgebases)
                {
                    var qnaEndpoint = new QnAMakerEndpoint()
                    {
                        KnowledgeBaseId = kb.KbId,
                        EndpointKey = kb.EndpointKey,
                        Host = kb.Hostname,
                    };

                    set.QnAConfiguration.Add(kb.Id, qnaEndpoint);
                }

                CognitiveModelSets.Add(language, set);
            }
        }

        public Dictionary<string, CognitiveModelSet> CognitiveModelSets { get; set; } = new Dictionary<string, CognitiveModelSet>();

        public CognitiveModelSet GetCognitiveModels()
        {
            // Get cognitive models for locale
            var locale = CultureInfo.CurrentUICulture.Name.ToLower();

            var cognitiveModel = CognitiveModelSets.ContainsKey(locale)
                ? CognitiveModelSets[locale]
                : CognitiveModelSets.Where(key => key.Key.StartsWith(locale.Substring(0, 2))).FirstOrDefault().Value
                ?? throw new Exception($"There's no matching locale for '{locale}' or its root language '{locale.Substring(0, 2)}'. " +
                                        "Please review your available locales in your cognitivemodels.json file.");

            return cognitiveModel;
        }
    }
}