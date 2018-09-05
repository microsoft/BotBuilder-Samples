// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TurnContext } = require('botbuilder');

class MainDialog {
    /**
     * 
     * @param {Storage} storage A storage system like MemoryStorage used to store information.
     * @param {BotAdapter} adapter A BotAdapter used to send and receive messages.
     */
    constructor (storage, adapter) {
        this.storage = storage;
        this.adapter = adapter;
    }

    /**
     * 
     * @param {TurnContext} context A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {

            const utterance = (context.activity.text || '').trim().toLowerCase();

            // If the user says "subscribe", extract the conversation reference
            // and set up a message to send 5 seconds later.
            if (utterance === 'subscribe') {
                const reference = TurnContext.getConversationReference(context.activity);
                const userId = await this.saveReference(reference);
                await this.subscribeUser(userId);
                await context.sendActivity(`Thank you! We will message you in 5 seconds.`);
            } else {
                await context.sendActivity(`Say "subscribe" to schedule an example proactive message.`);
            }

        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].name !== 'Bot') {
            // Send a "this is what the bot does" message.
            await context.sendActivity(`Say "subscribe" to schedule an example proactive message.`);
        }
    }

    // Store the conversation reference in the storage system by userId.
    async saveReference(reference) {
        const userId = reference.activityId;
        const changes = {};
        changes['reference/' + userId] = reference;
        await this.storage.write(changes);

        return userId;
    }

    // Load a stored conversation reference from the storage system.
    async findReference(userId) {
        const referenceKey = 'reference/' + userId;
        const rows = await this.storage.read([referenceKey]);
        const reference = await rows[referenceKey];

        return reference;
    }
    
    // Set up a proactive message to be delivered to a user with a 5 second delay.
    async subscribeUser(userId) {
        setTimeout(async () => {
            const reference = await this.findReference(userId);
            if (reference) {
                this.adapter.continueConversation(reference, async (context) => {
                    await context.sendActivity(`This is your proactive notication!`);
                });
            }
        }, 5000);

    }


}

module.exports = MainDialog;