// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class InterruptionDispatcher : ComponentDialog
    {
        private const string NONE_INTENT = "None";
        private const string INTERRUPTION_DISPATCHER_DIALOG = "interruptionDispatcherDialog";

        private IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        private ConversationState _conversationState;
        private IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private BotServices _botServices;

        /**
         * Constructor.
         *
         * @param {StatePropertyAccessor} onTurnAccessor
         * @param {ConversationState} conversationState
         * @param {StatePropertyAccessor} userProfileAccessor
         * @param {BotConfiguration} botConfig
         */
        public InterruptionDispatcher(
                    IStatePropertyAccessor<OnTurnProperty> onTurnAccessor,
                    ConversationState conversationState,
                    IStatePropertyAccessor<UserProfile> userProfileAccessor,
                    BotServices botServices)
            : base(INTERRUPTION_DISPATCHER_DIALOG)
        {
            _onTurnAccessor = onTurnAccessor ?? throw new ArgumentNullException(nameof(onTurnAccessor));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _userProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(userProfileAccessor));
            _botServices = botServices ?? throw new ArgumentNullException(nameof(botServices));

            // add dialogs
            AddDialog(new WhatCanYouDo());
            AddDialog(new QnADialog(botServices, userProfileAccessor));
        }

        /**
         * Override onDialogBegin
         *
         * @param {DialogContext} dc dialog context
         * @param {Object} options dialog turn options
         */

        protected async override Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Override default begin() logic with interruption orchestration logic
            return await InterruptionDispatch(innerDc, options);
        }

        /**
         * Override onDialogContinue
         *
         * @param {DialogContext} dialog context
         */
        protected async override Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Override default continue() logic with interruption orchestration logic
            return await InterruptionDispatch(innerDc, null);
        }

        /**
         * Helper method to dispatch on interruption.
         *
         * @param {DialogContext} dc
         * @param {Object} options
         */
        protected async Task<DialogTurnResult> InterruptionDispatch(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            // See if interruption is allowed
            var context = innerDc.Context;
            var results = options as OnTurnProperty;

            if (results == null)
            {
                await context.SendActivityAsync("Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one.");
                return await innerDc.EndDialogAsync();
            }

            switch (results.Intent)
            {
                // Help, ChitChat and QnA share the same QnA Maker model. So just call the QnA Dialog.
                case nameof(QnADialog):
                case nameof(ChitChatDialog):
                //case nameof(HelpDialog):
                    return await innerDc.BeginDialogAsync(nameof(QnADialog));
                case nameof(WhatCanYouDo):
                case nameof(WhoAreYouDialog):
                case nameof(BookTableDialog):
                    await context.SendActivityAsync("Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one");
                    return await innerDc.EndDialogAsync();
                default:
                    await context.SendActivityAsync("I'm still learning.. Sorry, I do not know how to help you with that.");
                    await context.SendActivityAsync($"Follow[this link](https://www.bing.com/search?q={context.Activity.Text}) to search the web!");
                    return new DialogTurnResult(DialogTurnStatus.Waiting, null);
            }
        }


    }
}
