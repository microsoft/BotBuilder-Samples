/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { TestAdapter, ActivityTypes, TurnContext, ConversationState, MemoryStorage, UserState } from 'botbuilder';
import { DialogSet, DialogTurnStatus, Dialog } from 'botbuilder-dialogs';
import { DialogAndWelcomeBot }  from '../../bots/dialogAndWelcomeBot';
const assert = require('assert');

/**
 * A simple mock for a root dialog that gets invoked by the bot.
 */
class MockRootDialog extends Dialog {
    constructor() {
        super('mockRootDialog');
    }

    async beginDialog(dc, options) {
        await dc.context.sendActivity(`${ this.id } mock invoked`);
        return await dc.endDialog();
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

describe('DialogAndWelcomeBot', () => {
    const testAdapter = new TestAdapter(async (context) => undefined);

    async function processActivity(activity, bot) {
        const context = new TurnContext(testAdapter, activity);
        await bot.run(context);
    }

    it('Shows welcome card on member added and starts main dialog', async () => {
        const mockRootDialog = new MockRootDialog();
        const memoryStorage = new MemoryStorage();
        const sut = new DialogAndWelcomeBot(new ConversationState(memoryStorage), new UserState(memoryStorage), mockRootDialog);

        // Create conversationUpdate activity
        const conversationUpdateActivity = {
            type: ActivityTypes.ConversationUpdate,
            channelId: 'test',
            conversation: {
                id: 'someId'
            },
            membersAdded: [
                { id: 'theUser' }
            ],
            recipient: { id: 'theBot' }
        };

        // Send the conversation update activity to the bot.
        await processActivity(conversationUpdateActivity, sut);

        // Assert we got the welcome card
        let reply = testAdapter.activityBuffer.shift();
        assert.strictEqual(reply.attachments.length, 1);
        assert.strictEqual(reply.attachments[0].contentType, 'application/vnd.microsoft.card.adaptive');

        // Assert that we started the main dialog.
        reply = testAdapter.activityBuffer.shift();
        assert.strictEqual(reply.text, 'mockRootDialog mock invoked');
    });
});
