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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
        private const string WelcomeText = "This bot will save a conversation transcript to Azure Blob Storage." +
                                           " Type anything to get started and the bot will echo back what you type." +
                                           " Type !history to save your transcript in Blob Storage.";

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

                    // WebChat and Emulator require modifying the activity.Id to display the same activity again within the same chat window
                    bool updateActivities = new[] { Channels.Webchat, Channels.Emulator, Channels.Directline, }.Contains(activity.ChannelId);
                    var incrementId = 0;
                    if (updateActivities && activity.Id.Contains("|"))
                    {
                        int.TryParse(activity.Id.Split('|')[1], out incrementId);
                    }

                    do
                    {
                        var pagedTranscript = await _transcriptStore.GetTranscriptActivitiesAsync(activity.ChannelId, activity.Conversation.Id, continuationToken);
                        var activities = pagedTranscript.Items
                            .Where(a => a.Type == ActivityTypes.Message)
                            .Select(ia => (Activity)ia)
                            .ToList();

                        if (updateActivities)
                        {
                            foreach (var a in activities)
                            {
                                incrementId++;
                                a.Id = string.Concat(activity.Conversation.Id, "|", incrementId.ToString().PadLeft(7, '0'));
                                a.Timestamp = DateTimeOffset.UtcNow;
                                a.ChannelData = string.Empty; // WebChat uses ChannelData for id comparisons, so we clear it here
                            }
                        }

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

                    await turnContext.SendActivityAsync("Transcript sent", cancellationToken: cancellationToken);
                }
                else
                {
                    // Echo back to the user whatever they typed.
                    await turnContext.SendActivityAsync($"You sent '{activity.Text}'\n", cancellationToken: cancellationToken);
                }
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to ConversationHistoryBot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}
