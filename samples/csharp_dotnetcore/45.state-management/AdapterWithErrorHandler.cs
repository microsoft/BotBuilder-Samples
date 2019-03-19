// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(ICredentialProvider credentialProvider, ILogger<BotFrameworkHttpAdapter> logger)
            : base(credentialProvider)
        {
            // Enable logging at the adapter level using OnTurnError.
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError($"Exception caught : {exception}");
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");

                // Depending on the application and how the state is being used you may also decide to
                // delete either the ConversationState or UserState or both when you get a leaked global exception.
                // This can avoid a bot getting "stuck" in an infinite loop prompting the user.
            };
        }
    }
}
