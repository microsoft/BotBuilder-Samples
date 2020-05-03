// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { Dialog, DialogReason } = require('botbuilder-dialogs');

// Custom dialogs might define their own custom state.
// Similarly to the Waterfall dialog we will have a set of values in the ConversationState. However, rather than persisting
// an index we will persist the last property we prompted for. This way when we resume this code following a prompt we will
// have remembered what property we were filling.
const SlotName = 'slot';
const PersistedValues = 'values';

class SlotFillingDialog extends Dialog {
    /**
     * This is an example of implementing a custom Dialog class. This is similar to the Waterfall dialog in the framework;
     * however, it is based on a Dictionary rather than a sequential set of functions. The dialog is defined by a list of 'slots',
     * each slot represents a property we want to gather and the dialog we will be using to collect it. Often the property
     * is simply an atomic piece of data such as a number or a date. But sometimes the property is itself a complex object, in which
     * case we can use the slot dialog to collect that compound property.
     * @param {string} dialogId A unique identifier for this dialog.
     * @param {Array} slots An array of SlotDetails that define the required slots.
     */
    constructor(dialogId, slots) {
        super(dialogId);

        if (!slots) throw new Error('[SlotFillingDialog]: Missing parameter. slots parameter is required');

        this.slots = slots;
    }

    /**
     * Begin is called to start a new slot dialog.
     * @param {DialogContext} dc A handle on the runtime.
     */
    async beginDialog(dc) {
        // Don't do anything for non-message and non-ConversationUpdate activities.
        if (dc.context.activity.type !== ActivityTypes.Message && dc.context.activity.type !== ActivityTypes.ConversationUpdate) {
            return dc.endDialog();
        }

        // Initialize a spot to store these values.
        dc.activeDialog.state[PersistedValues] = {};

        // Call runPrompt, which will find the next slot to fill.
        return await this.runPrompt(dc);
    }

    /**
     *  Continue is called to run an existing dialog. It will return the state of the current dialog. If there is no dialog it will return Empty.
     *  @param {DialogContext} dc A handle on the runtime.
     */
    async continueDialog(dc) {
        // Skip non-message activities.
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Run next step with the message text as the result.
        return await this.runPrompt(dc);
    }

    /**
     * Resume is called when a child dialog completes and we need to carry on processing in this class.
     * @param {DialogContext} dc
     * @param {DialogReason} reason
     * @param {object} result
     */
    async resumeDialog(dc, reason, result) {
        // Update the state with the result from the child prompt.
        const slotName = dc.activeDialog.state[SlotName];

        // Get the previously persisted values.
        const values = dc.activeDialog.state[PersistedValues];

        // Set the new value into the appropriate slot name.
        values[slotName] = result;

        // Move on to the next slot in the dialog.
        return await this.runPrompt(dc);
    }

    /**
     * This helper function contains the core logic of this dialog. The main idea is to compare the state we have gathered with the
     * list of slots we have been asked to fill. When we find an empty slot we execute the corresponding prompt.
     * @param {DialogContext} dc
     */
    async runPrompt(dc) {
        // runPrompt finds the next slot to fill, then calls the appropriate prompt to fill it.
        const state = dc.activeDialog.state;
        const values = state[PersistedValues];

        // Run through the list of slots until we find one that hasn't been filled yet.
        const unfilledSlot = this.slots.filter(function(slot) { return !Object.keys(values).includes(slot.name); });

        // If we have an unfilled slot we will try to fill it
        if (unfilledSlot.length) {
            state[SlotName] = unfilledSlot[0].name;

            // If the slot contains prompt text create the PromptOptions.

            // Run the child dialog
            return await dc.beginDialog(unfilledSlot[0].promptId, unfilledSlot[0].options);
        } else {
            // No more slots to fill so end the dialog.
            return await dc.endDialog(dc.activeDialog.state);
        }
    }
}

module.exports.SlotFillingDialog = SlotFillingDialog;
