// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.BotBuilderSamples.DialogRootBot.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Microsoft.BotBuilderSamples.DialogRootBot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ICredentialProvider credentialProvider, AuthenticationConfiguration authConfig, ILogger<AdapterWithErrorHandler> logger)
            : base(configuration, credentialProvider, authConfig, logger: logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                //logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync($"Sorry, it looks like something went wrong. \r\n{exception}");
            };

            Use(new LoggerMiddleware(logger));
        }
    }
}
