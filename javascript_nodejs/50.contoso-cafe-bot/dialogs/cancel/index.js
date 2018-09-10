// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, ConfirmPrompt, WaterfallDialog, DialogTurnStatus } = require('botbuilder-dialogs');

const turnResult = require('../shared/turnResult');

// Cancel intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CANCEL_DIALOG = 'Cancel';
const START_DIALOG = 'start';
const CONFIRM_CANCEL_PROMPT = 'confirmCancel';

class CancelDialog extends ComponentDialog {
    /**
     * Constructor.
     */
    constructor() {
        super(CANCEL_DIALOG);

        // Add dialogs.
        this.addDialog(new WaterfallDialog(START_DIALOG,[
            this.promptToConfirm,
            this.finalizeCancel
        ])); 

        this.addDialog(new ConfirmPrompt(CONFIRM_CANCEL_PROMPT));
    }

    async promptToConfirm(dc, step) {
        // prompt for confirmation to cancel
        return await dc.prompt(CONFIRM_CANCEL_PROMPT, `Are you sure you want to cancel?`);
    }

    async finalizeCancel(dc, step) {
        if (step.result) {
            // User confirmed.
            await dc.cancelAll();
            await dc.context.sendActivity(`Sure. I've cancelled that!`);
            return await dc.end();
        } else {
            // User rejected cancellation.
            await dc.context.sendActivity(`Ok..`);
            await dc.end();
            return new turnResult(DialogTurnStatus.complete, {reason: 'Abandon', payload: step.options})
        }
    }
 };

CancelDialog.Name = CANCEL_DIALOG;

module.exports = CancelDialog;