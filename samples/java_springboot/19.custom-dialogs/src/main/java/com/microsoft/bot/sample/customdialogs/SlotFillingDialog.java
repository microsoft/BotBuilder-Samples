// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.customdialogs;

import com.microsoft.bot.connector.Async;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.dialogs.DialogContext;
import com.microsoft.bot.dialogs.DialogInstance;
import com.microsoft.bot.dialogs.DialogReason;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.schema.ActivityTypes;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.concurrent.CompletableFuture;

/**
 * This is an example of implementing a custom Dialog class. This is similar to the Waterfall dialog
 * in the framework; however, it is based on a Dictionary rather than a sequential set of functions.
 * The dialog is defined by a list of 'slots', each slot represents a property we want to gather and
 * the dialog we will be using to collect it. Often the property is simply an atomic piece of data
 * such as a number or a date. But sometimes the property is itself a complex object, in which case
 * we can use the slot dialog to collect that compound property.
 */
public class SlotFillingDialog extends Dialog {

    // Custom dialogs might define their own custom state.
    // Similarly to the Waterfall dialog we will have a set of values in the ConversationState.
    // However, rather than persisting an index we will persist the last property we prompted for.
    // This way when we resume this code following a prompt we will have remembered what property
    // we were filling.
    private static final String SLOT_NAME = "slot";
    private static final String PERSISTED_VALUES = "values";

    // The list of slots defines the properties to collect and the dialogs to use to collect them.
    private List<SlotDetails> slots;

    public SlotFillingDialog(String withDialogId, List<SlotDetails> withSlots) {
        super(withDialogId);
        if (withSlots == null) {
            throw new IllegalArgumentException("slot details are required");
        }
        slots = withSlots;
    }

    /**
     * Called when the dialog is started and pushed onto the dialog stack.
     *
     * @param dc      The {@link DialogContext} for the current turn of conversation.
     * @param options Initial information to pass to the dialog.
     * @return If the task is successful, the result indicates whether the dialog is still active
     * after the turn has been processed by the dialog.
     */
    @Override
    public CompletableFuture<DialogTurnResult> beginDialog(DialogContext dc, Object options) {
        if (dc == null) {
            return Async.completeExceptionally(new IllegalArgumentException("DialogContext"));
        }

        // Don't do anything for non-message activities.
        if (!dc.getContext().getActivity().isType(ActivityTypes.MESSAGE)) {
            return dc.endDialog(new HashMap<String, Object>());
        }

        // Run prompt
        return runPrompt(dc);
    }

    /**
     * Called when the dialog is _continued_, where it is the active dialog and the user replies
     * with a new activity.
     *
     * <p>If this method is *not* overridden, the dialog automatically ends when the user
     * replies.</p>
     *
     * @param dc The {@link DialogContext} for the current turn of conversation.
     * @return If the task is successful, the result indicates whether the dialog is still active
     * after the turn has been processed by the dialog. The result may also contain a return value.
     */
    @Override
    public CompletableFuture<DialogTurnResult> continueDialog(
        DialogContext dc
    ) {
        if (dc == null) {
            return Async.completeExceptionally(new IllegalArgumentException("DialogContext"));
        }

        // Don't do anything for non-message activities.
        if (!dc.getContext().getActivity().isType(ActivityTypes.MESSAGE)) {
            return CompletableFuture.completedFuture(END_OF_TURN);
        }

        // Run next step with the message text as the result.
        return runPrompt(dc);
    }

    /**
     * Called when a child dialog completed this turn, returning control to this dialog.
     *
     * <p>Generally, the child dialog was started with a call to
     * {@link #beginDialog(DialogContext, Object)} However, if the {@link
     * DialogContext#replaceDialog(String, Object)} method is called, the logical child dialog may
     * be different than the original.</p>
     *
     * <p>If this method is *not* overridden, the dialog automatically ends when the user
     * replies.</p>
     *
     * @param dc     The dialog context for the current turn of the conversation.
     * @param reason Reason why the dialog resumed.
     * @param result Optional, value returned from the dialog that was called. The type of the value
     *               returned is dependent on the child dialog.
     * @return If the task is successful, the result indicates whether this dialog is still active
     * after this dialog turn has been processed.
     */
    @Override
    public CompletableFuture<DialogTurnResult> resumeDialog(
        DialogContext dc, DialogReason reason, Object result
    ) {
        if (dc == null) {
            return Async.completeExceptionally(new IllegalArgumentException("DialogContext"));
        }

        // Update the state with the result from the child prompt.
        String slotName = (String) dc.getActiveDialog().getState().get(SLOT_NAME);
        Map<String, Object> values = getPersistedValues(dc.getActiveDialog());
        values.put(slotName, result);

        // Run prompt
        return runPrompt(dc);
    }

    // This helper function contains the core logic of this dialog. The main idea is to compare
    // the state we have gathered with the list of slots we have been asked to fill. When we find
    // an empty slot we execute the corresponding prompt.
    private CompletableFuture<DialogTurnResult> runPrompt(DialogContext dc) {
        Map<String, Object> state = getPersistedValues(dc.getActiveDialog());

        // Run through the list of slots until we find one that hasn't been filled yet.
        Optional<SlotDetails> optionalSlot = slots.stream()
            .filter(item -> !state.containsKey(item.getName()))
            .findFirst();

        if (!optionalSlot.isPresent()) {
            // No more slots to fill so end the dialog.
            return dc.endDialog(state);
        } else {
            // If we have an unfilled slot we will try to fill it
            SlotDetails unfilledSlot = optionalSlot.get();

            // The name of the slot we will be prompting to fill.
            dc.getActiveDialog().getState().put(SLOT_NAME, unfilledSlot.getName());

            // Run the child dialog
            return dc.beginDialog(unfilledSlot.getDialogId(), unfilledSlot.getOptions());
        }
    }

    // Helper function to deal with the persisted values we collect in this dialog.
    private Map<String, Object> getPersistedValues(DialogInstance dialogInstance) {
        Map<String, Object> state = (Map<String, Object>) dialogInstance.getState()
            .get(PERSISTED_VALUES);
        if (state == null) {
            state = new HashMap<String, Object>();
            dialogInstance.getState().put(PERSISTED_VALUES, state);
        }
        return state;
    }
}
