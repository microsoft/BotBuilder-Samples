// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
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
        private readonly AzureBlobTranscriptStore _transcriptStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationHistoryBot"/> class.
        /// </summary>
        /// <param name="transcriptStore">Injected via ASP.NET dependency injection.</param>
        public ConversationHistoryBot(AzureBlobTranscriptStore transcriptStore)
        {
            _transcriptStore = transcriptStore ?? throw new ArgumentNullException(nameof(transcriptStore));
        }

        /// <summary>
        /// Every conversation turn will call this method. The method either echoes the message activity or uploads the history to the channel.
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
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var activity = turnContext.Activity;

            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Text == "!history")
                {
                    // Download the activities from the Transcript (blob store) and send them over to the channel when a request to upload history arrives.
                    // This could be an event or a special message acctivity as above.
                    var connectorClient = turnContext.TurnState.Get<ConnectorClient>(typeof(IConnectorClient).FullName);

                    // Get all the message type activities from the Transcript.
                    string continuationToken = null;
                    var count = 0;
                    do
                    {
                        var pagedTranscript = await _transcriptStore.GetTranscriptActivitiesAsync(activity.ChannelId, activity.Conversation.Id);
                        var activities = pagedTranscript.Items
                                            .Where(a => a.Type == ActivityTypes.Message)
                                            .Select(ia => (Activity)ia)
                                            .ToList();

                        // DirectLine only allows the upload of at most 500 activities at a time. The limit of 1500 below is
                        // arbitrary and up to the Bot author to decide.
                        count += activities.Count();
                        if (activities.Count() > 500 || count > 1500)
                        {
                            throw new InvalidOperationException("Attempt to upload too many activities");
                        }

                        var transcript = new Transcript(activities);

                        await connectorClient.Conversations.SendConversationHistoryAsync(activity.Conversation.Id, transcript, cancellationToken: cancellationToken);

                        continuationToken = pagedTranscript.ContinuationToken;
                    }
                    while (continuationToken != null);
                }
                else
                {
                    // Echo back to the user whatever they typed.
                    await turnContext.SendActivityAsync($"You sent '{activity.Text}'\n", cancellationToken: cancellationToken);
                }
            }
        }
    }
}
