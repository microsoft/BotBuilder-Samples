// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class ConversationHistoryBot : IBot
    {
        private readonly TranscriptStore _transcriptStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationHistoryBot"/> class.
        /// </summary>
        /// <param name="transcriptStore">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        public ConversationHistoryBot(TranscriptStore transcriptStore)
        {
            _transcriptStore = transcriptStore;
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Echo back to the user whatever they typed.
                var responseMessage = $"You sent '{turnContext.Activity.Text}'\n";
                await turnContext.SendActivityAsync(responseMessage);
            }

            // Send over the transcript when a request arrives. This is something that the client and the Bot have
            // to agree on.
            var shouldUploadHistory = false;
            if (shouldUploadHistory)
            {
                var connectorClient = turnContext.TurnState.Get<ConnectorClient>(typeof(IConnectorClient).FullName);
                var activity = turnContext.Activity;

                // Get all the message type activities from the Transcript.
                string continuationToken = null;
                var activities = new List<Activity>();
                do
                {
                    var pagedTranscript = await _transcriptStore.Store.GetTranscriptActivitiesAsync(activity.ChannelId, activity.Conversation.Id);
                    activities.AddRange(pagedTranscript.Items
                                        .Where(a => a.Type == ActivityTypes.Message)
                                        .Select(ia => (Activity)ia)
                                        .ToList());
                    continuationToken = pagedTranscript.ContinuationToken;
                }
                while (continuationToken != null);

                // Construct a Transcript object from the activities above and use the
                // SendConversationHistoryAsync API to upload the historic activities.
                var transcript = new Bot.Schema.Transcript(activities);
                await connectorClient.Conversations.SendConversationHistoryAsync(activity.Conversation.Id, transcript);
            }
        }
    }
}
