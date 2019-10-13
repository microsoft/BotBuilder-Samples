// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version __vX.X.X__

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, ConversationState conversationState = null)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                var errorMessageText = "The bot encounted an error or bug.";
                var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                await turnContext.SendActivityAsync(errorMessage);

                errorMessageText = "To continue to run this bot, please fix the bot source code.";
                errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                await turnContext.SendActivityAsync(errorMessage);

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
            };
                await SendTraceActivityAsync(turnContext, exception);
        }

        private static async Task SendTraceActivityAsync(ITurnContext turnContext, Exception exception)
        {
            // Only send a trace activity if we're talking to the Bot Framework Emulator
            if (turnContext.Activity.ChannelId == Channels.Emulator)
            {
                Activity traceActivity = new Activity(ActivityTypes.Trace)
                {
                    Label = "TurnError",
                    Name = "OnTurnError Trace",
                    Value = exception.Message,
                    ValueType = "https://www.botframework.com/schemas/error",
                };

                // Send a trace activity
                await turnContext.SendActivityAsync(traceActivity);
            }
        }
    }
}
