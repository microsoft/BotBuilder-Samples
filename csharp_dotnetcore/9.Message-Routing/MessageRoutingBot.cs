// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Options;

namespace MessageRoutingBot
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class MessageRoutingBot : IBot
    {
        private readonly BotServices _services;
        private readonly MessageRoutingBotAccessors _accessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingBot"/> class.
        /// </summary>
        /// <param name="botServices">Bot services.</param>
        /// <param name="accessors">Bot State Accessors.</param>
        public MessageRoutingBot(BotServices botServices, MessageRoutingBotAccessors accessors)
        {
            _services = botServices ?? throw new ArgumentNullException(nameof(botServices));
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            Dialogs = new DialogSet(accessors.ConversationDialogState);
            Dialogs.Add(new MainDialog(_services));
        }

        private DialogSet Dialogs { get; set; }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dc = await Dialogs.CreateContextAsync(turnContext);
            var result = await dc.ContinueAsync();

            if (result.Status == DialogTurnStatus.Empty)
            {
                // Start main dialog.
                await dc.BeginAsync(MainDialog.Name);
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Save the user profile updates into the user state.
            await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}