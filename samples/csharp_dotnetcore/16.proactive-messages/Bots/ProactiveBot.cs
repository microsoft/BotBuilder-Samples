// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class ProactiveBot<T> : IBot
        where T : Dialog
    {
        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        private readonly ConcurrentDictionary<string, ConversationReference> ConversationReferences;
        private readonly DialogManager DialogManager;
        private readonly ILogger Logger;

        public ProactiveBot(T rootDialog, ConcurrentDictionary<string, ConversationReference> conversationReferences, ILogger<ProactiveBot<T>> logger)
        {
            Logger = logger;
            ConversationReferences = conversationReferences;
            DialogManager = new DialogManager(rootDialog);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Running dialog with Activity.");

            // Add/update the conversation reference, so we can proactively message this conversation within NotifyController.
            AddConversationReference(turnContext.Activity as Activity);

            await DialogManager.OnTurnAsync(turnContext, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            ConversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }
    }
}
