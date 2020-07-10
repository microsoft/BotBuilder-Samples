// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Middleware;
using Microsoft.Bot.Solutions.Responses;
using Microsoft.Extensions.Logging;
using TaskModuleFactorySample.Extensions;
using TaskModuleFactorySample.Services;

namespace TaskModuleFactorySample.Adapters
{
    public class DefaultAdapter : BotFrameworkHttpAdapter
    {
        private readonly ConversationState _conversationState;
        private readonly ILogger _logger;
        private readonly IBotTelemetryClient _telemetryClient;
        private readonly LocaleTemplateManager _templateEngine;

        public DefaultAdapter(
            BotSettings settings,
            ICredentialProvider credentialProvider,
            IChannelProvider channelProvider,
            AuthenticationConfiguration authConfig,
            LocaleTemplateManager templateEngine,
            ConversationState conversationState,
            TelemetryInitializerMiddleware telemetryMiddleware,
            IBotTelemetryClient telemetryClient,
            ILogger<BotFrameworkHttpAdapter> logger)
            : base(credentialProvider, authConfig, channelProvider, logger: logger)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));

            OnTurnError = HandleTurnErrorAsync;

            Use(telemetryMiddleware);

            // Uncomment the following line for local development without Azure Storage
            // Use(new TranscriptLoggerMiddleware(new MemoryTranscriptStore()));
            Use(new TranscriptLoggerMiddleware(new AzureBlobTranscriptStore(settings.BlobStorage.ConnectionString, settings.BlobStorage.Container)));
            Use(new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true));
            Use(new ShowTypingMiddleware());
            Use(new SetLocaleMiddleware(settings.DefaultLocale ?? "en-us"));
            Use(new EventDebuggerMiddleware());
            Use(new SetSpeakMiddleware());
        }

        private async Task HandleTurnErrorAsync(ITurnContext turnContext, Exception exception)
        {
            // Log any leaked exception from the application.
            _logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

            await SendErrorMessageAsync(turnContext, exception);
            await SendEndOfConversationToParentAsync(turnContext, exception);
            await ClearConversationStateAsync(turnContext);
        }

        private async Task SendErrorMessageAsync(ITurnContext turnContext, Exception exception)
        {
            try
            {
                _telemetryClient.TrackException(exception);

                // Send a message to the user.
                await turnContext.SendActivityAsync(_templateEngine.GenerateActivityForLocale("ErrorMessage"));

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

        private async Task SendEndOfConversationToParentAsync(ITurnContext turnContext, Exception exception)
        {
            try
            {
                if (turnContext.IsSkill())
                {
                    // Send and EndOfConversation activity to the skill caller with the error to end the conversation
                    // and let the caller decide what to do.
                    var endOfConversation = Activity.CreateEndOfConversationActivity();
                    endOfConversation.Code = "SkillError";
                    endOfConversation.Text = exception.Message;
                    await turnContext.SendActivityAsync(endOfConversation);
                }
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