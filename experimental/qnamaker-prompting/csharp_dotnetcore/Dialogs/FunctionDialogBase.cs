// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Designer.Dialogs
{
    public abstract class FunctionDialogBase : Dialog
    {
        private const string FunctionStateName = "functionState";

        public FunctionDialogBase(string dialogId)
            : base(dialogId)
        {
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Don't do anything for non-message activities.
            if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return await dialogContext.EndDialogAsync().ConfigureAwait(false);
            }

            // Run dialog logic.
            return await RunStateMachineAsync(dialogContext, cancellationToken).ConfigureAwait(false);
        }

        public override async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Don't do anything for non-message activities.
            if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return EndOfTurn;
            }

            // Run dialog logic.
            return await RunStateMachineAsync(dialogContext, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task<(object newState, IEnumerable<Activity> output, object result)> ProcessAsync(object oldState, Activity input);

        private static object GetPersistedState(DialogInstance dialogInstance)
        {
            if (dialogInstance.State.TryGetValue(FunctionStateName, out var result))
            {
                return result;
            }
            return null;
        }

        private async Task<DialogTurnResult> RunStateMachineAsync(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            // Get the Process function's current state from the dialog state
            var oldState = GetPersistedState(dialogContext.ActiveDialog);

            // Run the Process function.
            var (newState, output, result) = await ProcessAsync(oldState, dialogContext.Context.Activity).ConfigureAwait(false);

            // If we have output to send then send it.
            foreach (var activity in output)
            {
                await dialogContext.Context.SendActivityAsync(activity).ConfigureAwait(false);
            }

            // If we have new state then we must still be running.
            if (newState != null)
            {
                // Add the state returned from the Process function to the dialog state.
                dialogContext.ActiveDialog.State[FunctionStateName] = newState;

                // Return Waiting indicating this dialog is still in progress.
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else
            {
                // The Process function indicates it's completed by returning null for the state.
                return await dialogContext.EndDialogAsync(result).ConfigureAwait(false);
            }
        }
    }
}
