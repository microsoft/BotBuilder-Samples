// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { Dialog, DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');
const { ActivityTypes } = require('botbuilder');

const functionStateName = 'functionState';

class FunctionDialogBase extends Dialog {
    
    constructor(dialogId) {
        super(dialogId);
    }

    async beginDialog(dc) {
        // Don't do anything for non-message activities.
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Run dialog logic.
        return await this.runStateMachine(dc);
    }

    async continueDialog(dc) {
        // Skip non-message activities.
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Run dialog logic.
        return await this.runStateMachine(dc);
    }

    getPersistedState(dialogInstance) {
        return dialogInstance.state[functionStateName];
    }

    async runStateMachine(dc) {
       
        var oldState = this.getPersistedState(dc.activeDialog);
        
        var processResult = await this.processAsync(oldState, dc.context.activity);
        
        var newState = processResult[0];
        var output = processResult[1];
        var result = processResult[2];
        
        await dc.context.sendActivity(output);

        if(newState != null){
            dc.activeDialog.state[functionStateName] = newState;
            return { status: DialogTurnStatus.waiting };
        }
        else{
            return await dc.endDialog();
        }
    }

    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }
}

module.exports.FunctionDialogBase = FunctionDialogBase;
