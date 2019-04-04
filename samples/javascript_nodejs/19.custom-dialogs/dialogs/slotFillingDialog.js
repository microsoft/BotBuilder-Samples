// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { Dialog } = require('botbuilder-dialogs');

const SlotName = 'slot';
const PersistedValues = 'values';

class SlotFillingDialog extends Dialog {
    /**
     * SlotFillingDialog is a Dialog class for offering slot filling features to a bot.
     * Given multiple slots to fill, the dialog will walk a user through all of them
     * until all slots are filled with user responses.
     * @param {string} dialogId A unique identifier for this dialog.
     * @param {Array} slots An array of SlotDetails that define the required slots.
     */
    constructor(dialogId, slots) {
        super(dialogId);
        this.slots = slots;
    }

    async beginDialog(dc) {
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Initialize a spot to store these values.
        dc.activeDialog.state[PersistedValues] = {};

        // Call runPrompt, which will find the next slot to fill.
        return await this.runPrompt(dc);
    }

    async continueDialog(dc) {
        // Skip non-message activities.
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Call runPrompt, which will find the next slot to fill.
        return await this.runPrompt(dc);
    }

    async resumeDialog(dc, reason, result) {
        // dialogResume is called whenever a prompt or child-dialog completes
        // and the parent dialog resumes.  Since every turn of a SlotFillingDialog
        // is a prompt, we know that whenever we resume, there is a value to capture.

        // The slotName of the slot that was just filled was been stored in the state.
        const slotName = dc.activeDialog.state[SlotName];

        // Get the previously persisted values.
        const values = dc.activeDialog.state[PersistedValues];

        // Set the new value into the appropriate slot name.
        values[slotName] = result;

        // Move on to the next slot in the dialog.
        return await this.runPrompt(dc);
    }

    async runPrompt(dc) {
        // runPrompt finds the next slot to fill, then calls the appropriate prompt to fill it.
        const state = dc.activeDialog.state;
        const values = state[PersistedValues];

        // Find unfilled slots by filtering the full list of slots, excluding those for which we already have a value.
        const unfilledSlot = this.slots.filter(function(slot) { return !Object.keys(values).includes(slot.name); });

        // If there are unfilled slots still left, prompt for the next one.
        if (unfilledSlot.length) {
            state[SlotName] = unfilledSlot[0].name;
            return await dc.prompt(unfilledSlot[0].promptId, unfilledSlot[0].options);
        } else {
            // If all the prompts are filled, we're done. Return the full state object,
            // which will now contain values for all the slots.
            return await dc.endDialog(dc.activeDialog.state);
        }
    }
}

module.exports.SlotFillingDialog = SlotFillingDialog;
