// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Cancel intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CANCEL_DIALOG = 'Cancel';
const START_DIALOG = 'start';

const { DialogTurnStatus, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');
const CancelState = require('./cancelState');

const BookTableDialog = require('../bookTable');
const dialogTurnResult = require('../shared/turnResult');
const cancelProperty = require('./cancelProperty');
class CancelDialog {
    constructor(activeDialogPropertyAccessor, onTurnPropertyAccessor) {
        super(CANCEL_DIALOG);
        if(!activeDialogPropertyAccessor) throw ('Need active dialog property accessor from Main Dialog');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor from Main Dialog');
        this.activeDialogPropertyAccessor = activeDialogPropertyAccessor;
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;
        // cancelState
        this.cancelState = new CancelState(conversationState);

        // add dialogs
        this.dialogs = new DialogSet(this.cancelState.cancelDialogPropertyAccessor);
        this.dialogs.add(new WaterfallDialog(START_DIALOG,[
            this.promptToConfirm,
            this.finalizeCancel
        ])); 
    }
    async promptToConfirm(dc, step) {
        // prompt if cancelState 
    }

    async finalizeCancel(dc, step) {

    }

    async onTurn(context, mainDC) {

        const activeDialog = await this.activeDialogPropertyAccessor.get(dc.context);
       
        if(activeDialog == undefined) {
            // there's nothing to cancel.
            await context.sendActivity(`Ok.`);
            return new dialogTurnResult(DialogTurnStatus.complete);
        }
        // multi-turn cancel is only an option for book table dialog.
        if(activeDialog !== BookTableDialog.Name) {
            // These multi-turn dialogs do not require confirmation to cancel.
            await mainDC.cancelAll();
            await context.sendActivity(`Sure. I've cancelled that`);
            return new dialogTurnResult(DialogTurnStatus.complete);
        } else { 
            // continue active dialog
            let dc = this.dialogs.createContext(context);
            let turnResult = dc.continue();

            if(!turnResult || turnResult.status === DialogTurnStatus.complete) return turnResult;

            // create and add cancel state property
            let cancelStateProperty = new cancelProperty(activeDialog);
            this.cancelState.cancelStatePropertyAccessor.set(context, cancelStateProperty);

            return await dc.begin(START_DIALOG);
        }
    }
 };

CancelDialog.Name = CANCEL_DIALOG;

module.exports = CancelDialog;