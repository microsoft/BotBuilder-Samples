// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LivePersonProxyBot;

namespace LivePersonConnector
{
    public class LivePersonAdapter : BotFrameworkHttpAdapter, ILivePersonAdapter
    {
        public LivePersonAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, HandoffMiddleware handoffMiddleware, LoggingMiddleware loggingMiddleware)
            : base(configuration, logger)
        {
            Use(handoffMiddleware);
            Use(loggingMiddleware);

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync($"The bot encounted an error '{exception.Message}'.").ConfigureAwait(false);

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError").ConfigureAwait(false);
            };
        }

        public async Task ProcessActivityAsync(Activity activity, string msAppId, ConversationReference conversationRef, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            BotAssert.ActivityNotNull(activity);

            activity.ApplyConversationReference(conversationRef, true);

            await ContinueConversationAsync(
                msAppId,
                conversationRef,
                (ITurnContext proactiveContext, CancellationToken ct) =>
                {
                    using (var contextWithActivity = new TurnContext(this, activity))
                    {
                        contextWithActivity.TurnState.Add(proactiveContext.TurnState.Get<IConnectorClient>());
                        return base.RunPipelineAsync(contextWithActivity, callback, cancellationToken);
                    }
                },
                cancellationToken).ConfigureAwait(false);
        }
    }
}
