// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, ConfirmPrompt, WaterfallDialog, DialogTurnStatus } = require('botbuilder-dialogs');
// Cancel intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CANCEL_DIALOG = 'Cancel';
const START_DIALOG = 'start';
const CONFIRM_CANCEL_PROMPT = 'confirmCancel';
const CANCEL_STATE_PROPERTY = 'cancelStateProperty';
const CANCEL_DIALOG_PROPERTY = 'cancelDialogProperty';
const turnResult = require('../shared/turnResult');
class CancelDialog extends ComponentDialog {
    constructor(activeDialogPropertyAccessor, onTurnPropertyAccessor, conversationState) {
        super(CANCEL_DIALOG);
        if(!activeDialogPropertyAccessor) throw ('Need active dialog property accessor from Main Dialog');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor from Main Dialog');
        this.activeDialogPropertyAccessor = activeDialogPropertyAccessor;
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;

        this.cancelStatePropertyAccessor = conversationState.createProperty(CANCEL_STATE_PROPERTY);
        this.cancelDialogPropertyAccessor = conversationState.createProperty(CANCEL_DIALOG_PROPERTY);

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
            await dc.cancelAll();
            await dc.context.sendActivity(`Sure. I've cancelled that!`);
            return await dc.end();
        } else {
            await dc.context.sendActivity(`Ok..`);
            await dc.end();
            return new turnResult(DialogTurnStatus.complete, {reason: 'Abandon', payload: step.options})
        }
    }

 };

CancelDialog.Name = CANCEL_DIALOG;

module.exports = CancelDialog;