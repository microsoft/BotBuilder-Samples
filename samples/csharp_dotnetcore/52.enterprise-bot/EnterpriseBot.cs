// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace EnterpriseBot
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class EnterpriseBot : IBot
    {
        private readonly BotServices _services;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnterpriseBot"/> class.
        /// </summary>
        /// <param name="botServices">Bot services.</param>
        /// <param name="conversationState">Bot conversation state.</param>
        /// <param name="userState">Bot user state.</param>
        public EnterpriseBot(BotServices botServices, ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));
            _services = botServices ?? throw new ArgumentNullException(nameof(botServices));

            _dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>(nameof(EnterpriseBot)));
            _dialogs.Add(new MainDialog(_services, _conversationState, _userState));
        }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext);
            var result = await dc.ContinueDialogAsync();

            if (result.Status == DialogTurnStatus.Empty)
            {
                await dc.BeginDialogAsync(nameof(MainDialog));
            }
        }
    }
}
