// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents a bot that processes incoming Activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// </summary>
    public class ScaleoutBot<T> : ActivityHandler where T : Dialog
    {
        private readonly IStore _store;
        private readonly Dialog _dialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleoutBot"/> class.
        /// </summary>
        /// <param name="store">The store we will be using. Created through dependency injection.</param>
        /// <param name="rootDialog">The root dialog to run. Created through the dependency injection.</param>
        public ScaleoutBot(IStore store, T dialog)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        }

        /// <summary>
        /// This bot runs Dialogs that send message Activites in a way that can be scaled out with a multi-machine deployment.
        /// The bot logic makes use of the standard HTTP ETag/If-Match mechanism for optimistic locking. This mechanism
        /// is commonly supported on cloud storage technologies from multiple vendors including teh Azure Blob Storage
        /// service. A full implementation against Azure Blob Storage is included in this sample.
        /// </summary>
        /// <param name="turnContext">The ITurnContext object created by the integration layer.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Create the storage key for this conversation.
            string key = $"{turnContext.Activity.ChannelId}/conversations/{turnContext.Activity.Conversation?.Id}";

            // The execution sits in a loop because there might be a retry if the save operation fails.
            while (true)
            {
                // Load any existing state associated with this key
                var (oldState, etag) = await _store.LoadAsync(key);

                // Run the dialog system with the old state and inbound activity, the result is a new state and outbound activities.
                var (activities, newState) = await DialogHost.RunAsync(_dialog, turnContext.Activity, oldState, cancellationToken);

                // Save the updated state associated with this key.
                bool success = await _store.SaveAsync(key, newState, etag);

                // Following a successful save, send any outbound Activities, otherwise retry everything.
                if (success)
                {
                    if (activities.Any())
                    {
                        // This is an actual send on the TurnContext we were given and so will actual do a send this time.
                        await turnContext.SendActivitiesAsync(activities, cancellationToken);
                    }

                    break;
                }
            }
        }
    }
}
