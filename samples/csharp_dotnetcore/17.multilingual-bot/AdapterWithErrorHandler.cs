// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.BotBuilderSamples.Translation;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(ICredentialProvider credentialProvider, ILogger<BotFrameworkHttpAdapter> logger, TranslationMiddleware translationMiddleware)
            : base(credentialProvider)
        {
            if (credentialProvider == null)
            {
                throw new NullReferenceException(nameof(credentialProvider));
            }

            if (logger == null)
            {
                throw new NullReferenceException(nameof(logger));
            }

            if (translationMiddleware == null)
            {
                throw new NullReferenceException(nameof(translationMiddleware));
            }

            // Add translation middleware to the adapter's middleware pipeline
            Use(translationMiddleware);
           
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
