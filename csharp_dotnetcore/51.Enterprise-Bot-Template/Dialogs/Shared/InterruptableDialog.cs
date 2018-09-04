// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public abstract class InterruptableDialog : ComponentDialog
    {
        public InterruptableDialog(string dialogId)
            : base(dialogId)
        {
            PrimaryDialogName = dialogId;
        }

        public string PrimaryDialogName { get; set; }

        protected override async Task<DialogTurnResult> OnDialogBeginAsync(DialogContext dc, DialogOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dc.Dialogs.Find(PrimaryDialogName) != null)
            {
                // Overrides default behavior which starts the first dialog added to the stack (i.e. Cancel waterfall)
                return await dc.BeginAsync(PrimaryDialogName);
            }
            else
            {
                // If we don't have a matching dialog, start the initial dialog
                return await dc.BeginAsync(InitialDialogId);
            }
        }

        protected override async Task<DialogTurnResult> OnDialogContinueAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            var status = await OnDialogInterruptionAsync(dc, cancellationToken);

            if (status == InterruptionStatus.Interrupted)
            {
                // Resume the waiting dialog after interruption
                await dc.RepromptAsync().ConfigureAwait(false);
                return EndOfTurn;
            }
            else if (status == InterruptionStatus.Waiting)
            {
                // Stack is already waiting for a response, shelve inner stack
                return EndOfTurn;
            }

            return await base.OnDialogContinueAsync(dc, cancellationToken);
        }

        protected abstract Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken);
    }
}