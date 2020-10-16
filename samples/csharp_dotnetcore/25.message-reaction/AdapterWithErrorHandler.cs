// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        private readonly ActivityLog _log;

        public AdapterWithErrorHandler(ActivityLog log, IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, IStorage storage, ConversationState conversationState)
            : base(configuration, logger)
        {
            _log = log;

            // These methods add middleware to the adapter. The middleware adds the storage and state objects to the
            // turn context each turn.
            this.UseStorage(storage);
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

        public async override Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {
            // We need to record the Activity Id from the Activity just sent in order to understand what future reactions are reactions too.
            // Since the server determines the id, we retrieve the id from the response and use it to log the activity.
            var sendResponses = await base.SendActivitiesAsync(turnContext, activities, cancellationToken);
            for (int i = 0; i < activities.Length; i++)
            {
                await _log.AppendAsync(sendResponses[i].Id, activities[i]).ConfigureAwait(false);
            }

            return sendResponses;
        }
    }
}
