// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.DialogSkillBot
{
    public class SkillAdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        private readonly ConversationState _conversationState;
        private readonly ILogger _logger;

        public SkillAdapterWithErrorHandler(IConfiguration configuration, ICredentialProvider credentialProvider, AuthenticationConfiguration authConfig, ILogger<BotFrameworkHttpAdapter> logger, ConversationState conversationState)
            : base(configuration, credentialProvider, authConfig, logger: logger)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            OnTurnError = HandleTurnError;
        }

        private async Task HandleTurnError(ITurnContext turnContext, Exception exception)
        {
            // Log any leaked exception from the application.
            _logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

            await SendErrorMessageAsync(turnContext, exception);
            await SendEoCToParentAsync(turnContext, exception);
            await ClearConversationStateAsync(turnContext);
        }

        private async Task SendErrorMessageAsync(ITurnContext turnContext, Exception exception)
        {
            try
            {
                // Send a message to the user.
                var errorMessageText = "The skill encountered an error or bug.";
                var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.IgnoringInput);
                await turnContext.SendActivityAsync(errorMessage);

                errorMessageText = "To continue to run this bot, please fix the bot source code.";
                errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
                await turnContext.SendActivityAsync(errorMessage);

                // Send a trace activity, which will be displayed in the Bot Framework Emulator.
                // Note: we return the entire exception in the value property to help the developer;
                // this should not be done in production.
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.ToString(), "https://www.botframework.com/schemas/error", "TurnError");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught in SendErrorMessageAsync : {ex}");
            }
        }

        private async Task SendEoCToParentAsync(ITurnContext turnContext, Exception exception)
        {
            try
            {
                // Send an EndOfConversation activity to the skill caller with the error to end the conversation,
                // and let the caller decide what to do.
                var endOfConversation = Activity.CreateEndOfConversationActivity();
                endOfConversation.Code = "SkillError";
                endOfConversation.Text = exception.Message;
                await turnContext.SendActivityAsync(endOfConversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught in SendEoCToParentAsync : {ex}");
            }
        }

        private async Task ClearConversationStateAsync(ITurnContext turnContext)
        {
            try
            {
                // Delete the conversationState for the current conversation to prevent the
                // bot from getting stuck in a error-loop caused by being in a bad state.
                // ConversationState should be thought of as similar to "cookie-state" for a Web page.
                await _conversationState.DeleteAsync(turnContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught on attempting to Delete ConversationState : {ex}");
            }
        }
    }
}
