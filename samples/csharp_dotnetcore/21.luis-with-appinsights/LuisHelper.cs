// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LuisBotAppInsights
{
    public static class LuisHelper
    {
        public static async Task<RecognizerResult> ExecuteLuisQuery(IBotTelemetryClient telemetryClient, IConfiguration configuration, ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            try
            {
                // Create the LUIS settings from configuration.
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://" + configuration["LuisAPIHostName"]);

                // TODO: fix the ambiguity introduced in 4.4.n
                // var recognizer = new LuisRecognizer(luisApplication, null, false, null);
                var recognizer = new TelemetryLuisRecognizer(telemetryClient, luisApplication, null, false, false, true);

                // The actual call to LUIS
                //var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);
                return await recognizer.RecognizeAsync(turnContext, true, cancellationToken);


            }
            catch (Exception e)
            {
                logger.LogWarning($"LUIS Exception: {e.Message} Check your LUIS configuration.");
            }

            return null;
        }
    }
}