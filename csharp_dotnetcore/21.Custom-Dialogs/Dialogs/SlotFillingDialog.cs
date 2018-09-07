// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class SlotFillingDialog : Dialog
    {
        private const string SlotName = "slot";
        private const string PersistedValues = "values";

        private List<SlotDetails> _slots;

        public SlotFillingDialog(string dialogId, List<SlotDetails> slots)
            : base(dialogId)
        {
            _slots = slots;
        }

        public override async Task<DialogTurnResult> DialogBeginAsync(DialogContext dialogContext, DialogOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext == null)
            {
                throw new ArgumentNullException(nameof(dialogContext));
            }

            // Don't do anything for non-message activities.
            if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return await dialogContext.EndAsync(new Dictionary<string, object>());
            }

            // Run prompt
            return await RunPromptAsync(dialogContext, cancellationToken);
        }

        public override async Task<DialogTurnResult> DialogContinueAsync(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext == null)
            {
                throw new ArgumentNullException(nameof(dialogContext));
            }

            // Don't do anything for non-message activities.
            if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return EndOfTurn;
            }

            // Run next step with the message text as the result.
            return await RunPromptAsync(dialogContext, cancellationToken);
        }

        public override async Task<DialogTurnResult> DialogResumeAsync(DialogContext dialogContext, DialogReason reason, object result, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext == null)
            {
                throw new ArgumentNullException(nameof(dialogContext));
            }

            // Update the state with the result from the child prompt. 
            var slotName = (string)dialogContext.ActiveDialog.State[SlotName];
            var values = GetPersistedValues(dialogContext.ActiveDialog);
            values[slotName] = result;

            // Run prompt.
            return await RunPromptAsync(dialogContext, cancellationToken);
        }

        private Task<DialogTurnResult> RunPromptAsync(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            var state = GetPersistedValues(dialogContext.ActiveDialog);

            var unfilledSlot = _slots.FirstOrDefault((item) => !state.ContainsKey(item.Name));

            // If we have an unfilled slot we will try to fill it
            if (unfilledSlot != null)
            {
                // The name of the slot we will be prompting to fill.
                dialogContext.ActiveDialog.State[SlotName] = unfilledSlot.Name;

                // If the slot contains prompt text create the PromptOptions.

                // Run the child dialog
                return dialogContext.BeginAsync(unfilledSlot.PromptId, CreatePromptOptions(unfilledSlot.PromptText), cancellationToken);
            }
            else
            {
                // No more slots to fill so end the dialog.
                return dialogContext.EndAsync(state);
            }
        }

        private static IDictionary<string, object> GetPersistedValues(DialogInstance dialogInstance)
        {
            object obj;
            if (!dialogInstance.State.TryGetValue(PersistedValues, out obj))
            {
                obj = new Dictionary<string, object>();
                dialogInstance.State.Add(PersistedValues, obj);
            }
            return (IDictionary<string, object>)obj;
        }

        private static PromptOptions CreatePromptOptions(string promptText)
        {
            return promptText != null ? new PromptOptions { Prompt = MessageFactory.Text(promptText) } : null;
        }
    }
}
