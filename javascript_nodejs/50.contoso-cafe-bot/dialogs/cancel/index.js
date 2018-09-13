// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, ConfirmPrompt, WaterfallDialog, DialogTurnStatus } = require('botbuilder-dialogs');
const { TurnResultHelper } = require('../shared/helpers');

// Cancel intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CANCEL_DIALOG = 'Cancel';
const START_DIALOG = 'start';
const CONFIRM_CANCEL_PROMPT = 'confirmCancelPrompt';

const CANCEL_PARENT = true;
const DO_NOT_CANCE_PARENT = false;

class CancelDialog extends ComponentDialog {
    /**
     * Constructor.
     */
    constructor() {
        super(CANCEL_DIALOG);

        // Add dialogs.
        this.addDialog(new WaterfallDialog(START_DIALOG,[
            this.promptToConfirm.bind(this),
            this.finalizeCancel.bind(this)
        ])); 

        // Add prompt.
        this.addDialog(new ConfirmPrompt(CONFIRM_CANCEL_PROMPT));
    }
    /**
     * Waterfall step for confirmation prompt.
     * 
     * @param {Object} dc dialog context
     * @param {Object} step 
     */
    async promptToConfirm(dc, step) {
        // prompt for confirmation to cancel
        return await dc.prompt(CONFIRM_CANCEL_PROMPT, `Are you sure you want to cancel?`);
    }
    /**
     * Waterfall step to finalize users response to the cancel prompt
     * 
     * @param {Object} dc dialog context
     * @param {Object} step 
     */
    async finalizeCancel(dc, step) {
        if (step.result) {
            // User confirmed.
            // Bubble up to parent that the user confirmed to cancel
            await dc.context.sendActivity(`Sure. I've cancelled that!`);
            await dc.cancelAll();
            return await dc.end(CANCEL_PARENT);
        } else {
            // User rejected.
            await dc.context.sendActivity(`Ok..`);
            await dc.end(DO_NOT_CANCE_PARENT);
            return new TurnResultHelper(DialogTurnStatus.complete, {reason: 'Abandon', payload: step.options})
        }
    }
    /**
     * 
     * @param {Object} outerDC outer dialog context
     * @param {object} result value passed to dc.end()
     */
    async endComponent(outerDC, result) {
        if(result === true) {
            // Cancel stacks in the parent.
            outerDC.cancelAll();
        }
    }
 };

CancelDialog.Name = CANCEL_DIALOG;

module.exports = CancelDialog;