// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, TurnContext } = require('botbuilder');
const { DialogSet, DialogTurnStatus } = require('botbuilder-dialogs');
const { DialogHostAdapter } = require('./dialogHostAdapter');
const { RefAccessor } = require('./refAccessor');

// The essential code for running a dialog. The execution of the dialog is treated here as a pure function call.
// The input being the existing (or old) state and the inbound Activity and the result being the updated (or new) state
// and the Activities that should be sent. The assumption is that this code can be re-run without causing any
// unintended or harmful side-effects, for example, any outbound service calls made directly from the
// dialog implementation should be idempotent.
class DialogHost {

    // A function to run a dialog while buffering the outbound Activities.
    static async run(rootDialog, activity, oldState) {

        // A custom adapter and corresponding TurnContext that buffers any messages sent.
        const adapter = new DialogHostAdapter();
        const turnContext = new TurnContext(adapter, activity);

        // Run the dialog using this TurnContext with the existing state.
        const newState = await DialogHost.runTurn(rootDialog, turnContext, oldState);

        // The result is a set of activities to send and a replacement state.
        return { activities: adapter.activities, newState: newState };
    }

    // Execute the turn of the bot. The functionality here closely resembles that which is found in the
    // IBot.OnTurnAsync method in an implementation that is using the regular BotFrameworkAdapter.
    // Also here in this example the focus is explicitly on Dialogs but the pattern could be adapted
    // to other conversation modeling abstractions.
    static async runTurn(rootDialog, turnContext, state) {

        if (turnContext.activity.type === ActivityTypes.Message) {

            // If we have some state, deserialize it. (This mimics the shape produced by BotState.cs.)
            const dialogStateProperty = state == undefined ? undefined : state['dialogState'];

            // A custom accessor is used to pass a handle on the state to the dialog system.
            const accessor = new RefAccessor(dialogStateProperty);

            // The following is regular dialog driver code.
            const dialogs = new DialogSet(accessor);
            dialogs.add(rootDialog);

            const dialogContext = await dialogs.createContext(turnContext);
            const results = await dialogContext.continueDialog();

            if (results.status == DialogTurnStatus.empty) {
                await dialogContext.beginDialog('root');
            }

            // Serialize the result (available as Value on the accessor).
            return { dialogState: accessor.value };
        }

        return state;
    }
}

module.exports.DialogHost = DialogHost;
