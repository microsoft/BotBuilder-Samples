const { ActivityTypes } = require('botbuilder-core');
const { Dialog, DialogReason } = require('botbuilder-dialogs');

const SlotName = "slot";
const PersistedValues = "values";


class SlotFillingDialog extends Dialog {

    constructor (dialogId, slots) {

        super(dialogId);
        this.slots = slots;
    }

    async dialogBegin(dc, options) {
        // Initialize the state
        const state = dc.activeDialog.state;
        state.options = options || {};
        state.values = {};

        if (dc.context.activity.type !== ActivityTypes.Message) {
            return await Dialog.EndOfTurn;
        }


        // Run the first step
        return await this.runPrompt(dc);
    }

    async dialogContinue(dc) {
        // Don't do anything for non-message activities
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        return await this.runPrompt(dc);
    }

    async dialogResume(dc, reason, result) {
        // Update the state with the result from the child prompt.
        const slotName = dc.activeDialog.state[SlotName];
        const values = dc.activeDialog.state[PersistedValues];
        values[slotName] = result;  
        return await this.runPrompt(dc);
    }

    async runPrompt(dc) {
        const state = dc.activeDialog.state;
        const values = state[PersistedValues];
        
        const unfilledSlot = this.slots.filter(function(slot) { return !Object.keys(values).includes(slot.Name); });

        if (unfilledSlot.length) {
            state[SlotName] = unfilledSlot[0].Name;

            return await dc.prompt(unfilledSlot[0].PromptId, unfilledSlot[0].Options);
        } else {
            return await dc.end(dc.activeDialog.state);
        }
    }
}

module.exports = SlotFillingDialog;