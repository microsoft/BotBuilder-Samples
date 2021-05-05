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
    private final String slotName = "slot";
    private final String persistedValues = "values";

    // The list of slots defines the properties to collect and the dialogs to use to collect them.
    private final List<SlotDetails> slots;

    public SlotFillingDialog(String withDialogId, List<SlotDetails> withSlots) {
        super(withDialogId);
        if (withSlots == null) {
            throw new IllegalArgumentException("slot details are required");
        }
        slots = withSlots;
    }

    /**
     * Begin is called to start a new slot dialog.
     *
     * @param dc      A handle on the runtime.
     * @param options This isn't used in this implementation but required for the contract.
     * Potentially it could be used to pass in existing state - already filled slots for example.
     * @return A DialogTurnResult indicating the state of this dialog to the caller.
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
     * Continue is called to run an existing dialog.
     * It will return the state of the current dialog. If there is no dialog it will return Empty.
     *
     * @param dc A handle on the runtime.
     * @return A DialogTurnResult indicating the state of this dialog to the caller.
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
     * Resume is called when a child dialog completes and we need to carry on processing in this class.
     *
     * @param dc     A handle on the runtime.
     * @param reason The reason we have control back in this dialog.
     * @param result The result from the child dialog. For example this is the value from a prompt.
     * @return A DialogTurnResult indicating the state of this dialog to the caller.
     */
    @Override
    public CompletableFuture<DialogTurnResult> resumeDialog(
        DialogContext dc, DialogReason reason, Object result
    ) {
        if (dc == null) {
            return Async.completeExceptionally(new IllegalArgumentException("DialogContext"));
        }

        // Update the state with the result from the child prompt.
        String nameOfSlot = (String) dc.getActiveDialog().getState().get(slotName);
        Map<String, Object> values = getPersistedValues(dc.getActiveDialog());
        values.put(nameOfSlot, result);

        // Run prompt.
        return runPrompt(dc);
    }

    /**
     * Helper function to deal with the persisted values we collect in this dialog.
     *
     * @param dialogInstance A handle on the runtime instance associated with this dialog, the State is a property.
     * @return A dictionary representing the current state or a new dictionary if we have none.
     */
    private Map<String, Object> getPersistedValues(DialogInstance dialogInstance) {
        Map<String, Object> state = (Map<String, Object>) dialogInstance.getState()
                .get(persistedValues);
        if (state == null) {
            state = new HashMap<String, Object>();
            dialogInstance.getState().put(persistedValues, state);
        }
        return state;
    }

    /**
     * This helper function contains the core logic of this dialog.
     * The main idea is to compare the state we have gathered with the list of slots we have been asked to fill.
     * When we find an empty slot we execute the corresponding prompt.
     *
     * @param dc A handle on the runtime.
     * @return A DialogTurnResult indicating the state of this dialog to the caller.
     */
    private CompletableFuture<DialogTurnResult> runPrompt(DialogContext dc) {
        Map<String, Object> state = getPersistedValues(dc.getActiveDialog());

        // Run through the list of slots until we find one that hasn't been filled yet.
        Optional<SlotDetails> optionalSlot = slots.stream()
            .filter(item -> !state.containsKey(item.getName()))
            .findFirst();

        // If we have an unfilled slot we will try to fill it
        if (optionalSlot.isPresent()) {
            SlotDetails unfilledSlot = optionalSlot.get();

            // The name of the slot we will be prompting to fill.
            dc.getActiveDialog().getState().put(slotName, unfilledSlot.getName());

            // If the slot contains prompt text create the PromptOptions.

            // Run the child dialog
            return dc.beginDialog(unfilledSlot.getDialogId(), unfilledSlot.getOptions());
        } else {
            // No more slots to fill so end the dialog.
            return dc.endDialog(state);
        }
    }
}
