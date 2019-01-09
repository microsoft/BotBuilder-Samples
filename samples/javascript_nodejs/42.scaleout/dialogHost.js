// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, TurnContext } = require('botbuilder');
const { DialogSet, DialogTurnStatus } = require('botbuilder-dialogs');
const { DialogHostAdapter } = require('./dialogHostAdapter');
const { RefAccessor } = require('./refAccessor');

class DialogHost {

    static async run(rootDialog, activity, oldState) {

        // A custom adapter and corresponding TurnContext that buffers any messages sent.
        const adapter = new DialogHostAdapter();
        const turnContext = new TurnContext(adapter, activity);

        // Run the dialog using this TurnContext with the existing state.
        const newState = await DialogHost.runTurn(rootDialog, turnContext, oldState);

        // The result is a set of activities to send and a replacement state.
        return { activities: adapter.activities, newState: newState };
    }

    static async runTurn(rootDialog, turnContext, state) {

        if (turnContext.activity.type === ActivityTypes.Message) {

            let dialogStateProperty = state == undefined ? undefined : state['dialogState'];

            //dialogStateProperty = DialogHost.debug(turnContext, dialogStateProperty);

            const accessor = new RefAccessor(dialogStateProperty);

            const dialogs = new DialogSet(accessor);
            dialogs.add(rootDialog);

            const dialogContext = await dialogs.createContext(turnContext);
            const results = await dialogContext.continueDialog();

            if (results.status == DialogTurnStatus.empty) {
                await dialogContext.beginDialog("root");
            }

            return { dialogState: accessor.value };
        }

        return state;
    }

    static async debug(turnContext, dialogStateProperty) {
        if (dialogStateProperty == undefined) {
            dialogStateProperty = 1;
        }
        else {
            dialogStateProperty += 1;
        }
        const msg = `dialogStateProperty: ${dialogStateProperty}`;
        await turnContext.sendActivity({ type: ActivityTypes.Message, text: msg });
        return dialogStateProperty;
    }
}

module.exports.DialogHost = DialogHost;
