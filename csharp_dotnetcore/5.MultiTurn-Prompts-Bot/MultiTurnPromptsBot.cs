// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace MultiTurn_Prompts_Bot
{
    /// <summary>
    /// This bot illustrates a multi-turn scenario. Following a prompt for name it prompts for age.
    /// </summary>
    public class MultiTurnPromptsBot : IBot
    {
        private DialogSet _dialogs;

        public MultiTurnPromptsBot(BotAccessors accessors)
        {
            _dialogs = new DialogSet(accessors.ConversationDialogState);
            _dialogs.Add(new WaterfallDialog("details", new WaterfallStep[] { NameStep, AgeStep, FinalStep }));
            _dialogs.Add(new TextPrompt("name"));
            _dialogs.Add(new NumberPrompt<int>("age"));
        }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // We are only interested in Message Activities.
            if (turnContext.Activity.Type != ActivityTypes.Message)
            {
                return;
            }

            // Run the DialogSet - let the framework identify the current state of the dialog from 
            // the dialog stack and figure out what (if any) is the active dialog.
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueAsync(cancellationToken);

            // If the DialogTurnStatus is Empty we should start a new dialog.
            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginAsync("details");
            }
        }

        private static async Task<DialogTurnResult> NameStep(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }
        private static async Task<DialogTurnResult> AgeStep(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await dc.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."));
            return await dc.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("Please enter your age") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> FinalStep(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await dc.Context.SendActivityAsync(MessageFactory.Text($"I have your age as {stepContext.Result}."));
            return await dc.EndAsync();
        }
    }
}
