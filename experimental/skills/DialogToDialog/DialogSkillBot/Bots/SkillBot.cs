// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.Bots
{
    public class SkillBot<T> : IBot
        where T : Dialog
    {
        private readonly ConversationState _conversationState;
        private readonly Dialog _mainDialog;

        public SkillBot(ConversationState conversationState, T mainDialog)
        {
            _conversationState = conversationState;
            _mainDialog = mainDialog;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var dialogSet = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState")) { TelemetryClient = _mainDialog.TelemetryClient };
            dialogSet.Add(_mainDialog);

            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken).ConfigureAwait(false);
            if (turnContext.Activity.Type == ActivityTypes.EndOfConversation && dialogContext.Stack.Any())
            {
                // Handle remote cancellation request if we have something in the stack.
                var activeDialogContext = GetActiveDialogContext(dialogContext);

                // Send cancellation message to the top dialog in the stack to ensure all the parents are canceled in the right order. 
                await activeDialogContext.CancelAllDialogsAsync(true, cancellationToken: cancellationToken);
                var remoteCancelText = "**SkillBot.** The current mainDialog in the skill was **canceled** by a request **from the host**, do some cleanup here if needed.";
                await turnContext.SendActivityAsync(MessageFactory.Text(remoteCancelText, inputHint: InputHints.IgnoringInput), cancellationToken);
            }
            else
            {
                // Run the Dialog with the new message Activity and capture the results so we can send end of conversation if needed.
                var result = await dialogContext.ContinueDialogAsync(cancellationToken).ConfigureAwait(false);
                if (result.Status == DialogTurnStatus.Empty)
                {
                    var startMessageText = $"**SkillBot.** Starting {_mainDialog.Id}.";
                    await turnContext.SendActivityAsync(MessageFactory.Text(startMessageText, inputHint: InputHints.IgnoringInput), cancellationToken);
                    result = await dialogContext.BeginDialogAsync(_mainDialog.Id, null, cancellationToken).ConfigureAwait(false);
                }

                // Send end of conversation if it is complete
                if (result.Status == DialogTurnStatus.Complete || result.Status == DialogTurnStatus.Cancelled)
                {
                    var endMessageText = "**SkillBot.** The mainDialog in the skill has **completed**. Sending EndOfConversation.";
                    await turnContext.SendActivityAsync(MessageFactory.Text(endMessageText, inputHint: InputHints.IgnoringInput), cancellationToken);

                    // Send End of conversation at the end.
                    var activity = new Activity(ActivityTypes.EndOfConversation) { Value = result.Result };
                    await turnContext.SendActivityAsync(activity, cancellationToken);
                }
            }

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        // Recursively walk up the DC stack to find the active DC.
        private DialogContext GetActiveDialogContext(DialogContext dialogContext)
        {
            var child = dialogContext.Child;
            if (child == null)
            {
                return dialogContext;
            }

            return GetActiveDialogContext(child);
        }
    }
}
