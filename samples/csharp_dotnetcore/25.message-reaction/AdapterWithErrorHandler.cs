// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<IBotFrameworkHttpAdapter> logger)
            : base(configuration, httpClientFactory, logger)
        { 
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                // NOTE: In production environment, you should consider logging this to
                // Azure Application Insights. Visit https://aka.ms/bottelemetry to see how
                // to add telemetry capture to your bot.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
