// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithInspection : BotFrameworkHttpAdapter
    {
        public AdapterWithInspection(IConfiguration configuration, InspectionState inspectionState, UserState userState, ConversationState conversationState, IStorage storage, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            // Inspection needs credentiaols because it will be sending the Activities and User and Conversation State to the emulator
            var credentials = new MicrosoftAppCredentials(configuration["MicrosoftAppId"], configuration["MicrosoftAppPassword"]);
            Use(new InspectionMiddleware(inspectionState, userState, conversationState, credentials));

            // These methods add middleware to the adapter. The middleware adds the storage and state objects to the
            // turn context each turn.
            this.UseStorage(storage);
            this.UseBotState(userState);
            this.UseBotState(conversationState);

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
