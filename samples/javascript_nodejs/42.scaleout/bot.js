// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogHost } = require('./dialogHost');

class ScaleoutBot {

    constructor(store, rootDialog) {
        this.store = store;
        this.rootDialog = rootDialog;
    }

    /**
     *
     * @param {TurnContext} on turn context object.
     */
    async onTurn(turnContext) {

        // Create the storage key for this conversation.
        const key = `${turnContext.activity.channelId}/conversations/${turnContext.activity.conversation.id}`;

        // The execution sits in a loop because there might be a retry if the save operation fails.
        while (true) {

            // Load any existing state associated with this key
            const oldState = await this.store.load(key);

            // Run the dialog system with the old state and inbound activity, the result is a new state and outbound activities.
            const result = await DialogHost.run(this.rootDialog, turnContext.activity, oldState.value);

            // Save the updated state associated with this key.
            const success = await this.store.save(key, result.newState, oldState.eTag);

            // Following a successful save, send any outbound Activities, otherwise retry everything.
            if (success) {

                if (result.activities.length > 0) {

                    // This is an actual send on the TurnContext we were given and so will actual do a send this time.
                    await turnContext.sendActivities(result.activities);
                }

                break;
            }
        }
    }
}

module.exports.ScaleoutBot = ScaleoutBot;
