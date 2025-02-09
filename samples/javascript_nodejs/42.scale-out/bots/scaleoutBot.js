// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { MemoryStore } = require('../memoryStore');
const { DialogHost } = require('../dialogHost');

class ScaleoutBot extends ActivityHandler {
    /**
     *
     * @param {Dialog} dialog
     */
    constructor(dialog) {
        super();
        if (!dialog) throw new Error('[ScaleoutBot]: Missing parameter. dialog is required');

        this.dialog = dialog;

        this.onMessage(async (context, next) => {
            // Create the storage key for this conversation.
            const key = `${ context.activity.channelId }/conversations/${ context.activity.conversation?.id }`;

            var store = new MemoryStore();
            var dialogHost = new DialogHost();

            // The execution sits in a loop because there might be a retry if the save operation fails.
            while (true) {
                // Load any existing state associated with this key
                const { oldState, etag } = await store.loadAsync(key);

                // Run the dialog system with the old state and inbound activity, the result is a new state and outbound activities.
                const { activities, newState } = await dialogHost.runAsync(this.dialog, context.activity, oldState);

                // Save the updated state associated with this key.
                const success = await store.saveAsync(key, newState, etag);

                // Following a successful save, send any outbound Activities, otherwise retry everything.
                if (success) {
                    if (activities.length > 0) {
                        // This is an actual send on the TurnContext we were given and so will actual do a send this time.
                        await context.sendActivities(activities);
                    }

                    break;
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.ScaleoutBot = ScaleoutBot;
