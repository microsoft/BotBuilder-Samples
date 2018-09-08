// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Abstract class that represents a special kind of dialog that supports interruptions.
    /// This can be used for simple scenarios such as help interruption or cancellation, or
    /// for more complex custom scenarios.
    /// </summary>
    public abstract class InterruptableDialog : ComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptableDialog"/> class.
        /// </summary>
        /// <param name="dialogId">Id of the dialog.</param>
        public InterruptableDialog(string dialogId)
            : base(dialogId)
        {
            PrimaryDialogName = dialogId;
        }

        /// <summary>
        /// Gets or sets the primary name of the dialog.
        /// </summary>
        public string PrimaryDialogName { get; set; }

        /// <summary>
        /// Begin dialog execution.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="options">The <see cref="DialogOptions"/> for dialog execution.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task<DialogTurnResult>"/> representing the result of the dialog turn.</returns>
        protected override async Task<DialogTurnResult> OnDialogBeginAsync(DialogContext dc, DialogOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dc.Dialogs.Find(PrimaryDialogName) != null)
            {
                // Overrides default behavior which starts the first dialog added to the stack (i.e. Cancel waterfall).
                return await dc.BeginAsync(PrimaryDialogName);
            }
            else
            {
                // If we don't have a matching dialog, start the initial dialog.
                return await dc.BeginAsync(InitialDialogId);
            }
        }

        /// <summary>
        /// Continue dialog execution.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task<DialogTurnResult>"/> representing the result of the dialog turn.</returns>
        protected override async Task<DialogTurnResult> OnDialogContinueAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            var status = await OnDialogInterruptionAsync(dc, cancellationToken);

            if (status == InterruptionStatus.Interrupted)
            {
                // Resume the waiting dialog after interruption.
                await dc.RepromptAsync().ConfigureAwait(false);
                return EndOfTurn;
            }
            else if (status == InterruptionStatus.Waiting)
            {
                // Stack is already waiting for a response, shelve inner stack.
                return EndOfTurn;
            }

            return await base.OnDialogContinueAsync(dc, cancellationToken);
        }

        /// <summary>
        /// Handle dialog interruption. Left for subclasses to implement.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task<DialogTurnResult>"/> representing the result of the dialog turn.</returns>
        protected abstract Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken);
    }
}