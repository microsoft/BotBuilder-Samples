// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, ConfirmPrompt, WaterfallDialog } = require('botbuilder-dialogs');
// Cancel intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CANCEL_DIALOG = 'Cancel';
const START_DIALOG = 'start';
const CONFIRM_CANCEL_PROMPT = 'confirmCancel';
const CANCEL_STATE_PROPERTY = 'cancelStateProperty';
const CANCEL_DIALOG_PROPERTY = 'cancelDialogProperty';

const cancelProperty = require('../shared/stateProperties/cancelProperty');

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

        this.addDialog(new ConfirmPrompt(CONFIRM_CANCEL_PROMPT, this.confirmPromptValidator));
    }

    async confirmPromptValidator (turnContext, validatorContext) {
        let result = validatorContext.Recognized.Value; 

        // TODO: increment turn counter and decide if we should re-prompt
    }
    async promptToConfirm(dc, step) {
        // prompt if cancelState 
        const activeDialog = await this.activeDialogPropertyAccessor.get(dc.context);
        this.cancelStatePropertyAccessor.set(new cancelProperty(activeDialog));

        // prompt for confirmation to cancel
        return await dc.prompt(CONFIRM_DELETE_PROMPT, `Are you sure you want to cancel? ${activeDialog}`);
    }

    async finalizeCancel(dc, step) {
        if (step.result) {
            await dc.cancelAll();
            await dc.context.sendActivity(`Sure. I've cancelled that!`);
        } else {
            await dc.context.sendActivity(`Ok..`);
        }
        return await dc.end();
    }

 };

CancelDialog.Name = CANCEL_DIALOG;

module.exports = CancelDialog;