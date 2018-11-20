// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace BasicBot
{
    /// <summary>
    /// Abstract class that represents a dialog that supports interruptions.
    /// This can be used for scenarios such as help interruption or cancellation, or
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
        }

        /// <summary>
        /// Continue dialog execution.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task"/> representing the result of the dialog turn.</returns>
        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            var status = await OnDialogInterruptionAsync(dc, cancellationToken);

            if (status == InterruptionStatus.Interrupted)
            {
                // Resume the waiting dialog after interruption.
                await dc.RepromptDialogAsync().ConfigureAwait(false);
                return EndOfTurn;
            }
            else if (status == InterruptionStatus.Waiting)
            {
                // Stack is already waiting for a response, shelve inner stack.
                return EndOfTurn;
            }

            return await base.OnContinueDialogAsync(dc, cancellationToken);
        }

        /// <summary>
        /// Handle dialog interruption. Left for subclasses to implement.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task"/> representing the result of the dialog turn.</returns>
        protected abstract Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken);
    }
}
