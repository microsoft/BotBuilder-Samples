// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TurnContext } = require('botbuilder-core');
const { DialogHostAdapter } = require('./dialogHostAdapter');
const { RefAccessor } = require('./refAccessor');

/**
 * The essential code for running a dialog. The execution of the dialog is treated here as a pure function call.
 * The input being the existing (or old) state and the inbound Activity and the result being the updated (or new) state
 * and the Activities that should be sent. The assumption is that this code can be re-run without causing any
 * unintended or harmful side-effects, for example, any outbound service calls made directly from the
 * dialog implementation should be idempotent.
 */
class DialogHost {
    /**
     * A function to run a dialog while buffering the outbound Activities.
     * @param {Dialog} dialog The dialog to run.
     * @param {IMessageActivity} activity The inbound Activity to run it with.
     * @param {JObject} oldState The existing or old state.
     * @returns {Array} An array of Activities 'sent' from the dialog as it executed. And the updated or new state.
     */
    async runAsync(dialog, activity, oldState) {
        // A custom adapter and corresponding TurnContext that buffers any messages sent.
        const adapter = new DialogHostAdapter();
        const turnContext = new TurnContext(adapter, activity);

        // Run the dialog using this TurnContext with the existing state.
        const newState = await this.runTurnAsync(dialog, turnContext, oldState);

        // The result is a set of activities to send and a replacement state.
        return [adapter.activities.toArray(), newState];
    }

    /**
     * Execute the turn of the bot. The functionality here closely resembles that which is found in the
     * IBot.OnTurnAsync method in an implementation that is using the regular BotFrameworkAdapter.
     * Also here in this example the focus is explicitly on Dialogs but the pattern could be adapted
     * to other conversation modeling abstractions.
     * @param {Dialog} dialog The dialog to be run.
     * @param {ITurnContext} turnContext The ITurnContext instance to use. Note this is not the one passed into the IBot OnTurnAsync.
     * @param {JObject} state The existing or old state of the dialog.
     * @returns {JObject} The updated or new state of the dialog.
     */
    async runTurnAsync(dialog, turnContext, state) {
        // If we have some state, deserialize it. (This mimics the shape produced by BotState.cs.)
        const dialogStateProperty = state?.DialogState;
        const dialogState = dialogStateProperty ? JSON.parse(dialogStateProperty) : undefined;
        // const dialogState = dialogStateProperty ? StateJsonSerializer.toObject(dialogStateProperty, StateJsonSerializer) : null;

        // A custom accessor is used to pass a handle on the state to the dialog system.
        const accessor = new RefAccessor(dialogState);

        // Run the dialog.
        await dialog.runAsync(turnContext, accessor);

        // Serialize the result (available as Value on the accessor), and put its value back into a new JObject.
        return { DialogState: JSON.stringify(accessor.value) };
    }
}

module.exports.DialogHost = DialogHost;
