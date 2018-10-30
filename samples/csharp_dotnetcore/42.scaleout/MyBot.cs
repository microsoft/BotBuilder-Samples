// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ScaleoutBot
{
    public class MyBot : IBot
    {
        private IStore _store;
        private Dialog _rootDialog;

        public MyBot(IStore store, Dialog rootDialog)
        {
            _store = store;
            _rootDialog = rootDialog;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create the storage key for this conversation.
            string key = $"{turnContext.Activity.ChannelId}/conversations/{turnContext.Activity.Conversation?.Id}";

            // The execution sits in a loop because there might be a retry if the save operation fails.
            while (true)
            {
                // Load any existing state associated with this key
                var (oldState, etag) = await _store.LoadAsync(key);

                // Run the dialog system with the old state and inbound activity, the result is a new state and outbound activities.
                var (activities, newState) = await DialogHost.RunAsync(_rootDialog, turnContext.Activity, oldState);

                // Save the updated state associated with this key.
                bool success = await _store.SaveAsync(key, newState, etag);

                // Following a successful save, send any outbound Activities, otherwise retry everything.
                if (success)
                {
                    if (activities.Any())
                    {
                        // This is an actual send on the TurnContext we were given and so will actual do a send this time.
                        await turnContext.SendActivitiesAsync(activities);
                    }
                    break;
                }
            }
        }
    }
}
