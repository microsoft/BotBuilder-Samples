/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { MessageFactory } from 'botbuilder';
import { DialogTestClient, DialogTestLogger } from 'botbuilder-testing';
import { TextPrompt, WaterfallDialog } from 'botbuilder-dialogs';
import { CancelAndHelpDialog } from '../../dialogs/cancelAndHelpDialog';
const assert = require('assert');

/**
 * An waterfall dialog derived from CancelAndHelpDialog for testing
 */
class TestCancelAndHelpDialog extends CancelAndHelpDialog {
    constructor() {
        super('TestCancelAndHelpDialog');

        this.addDialog(new TextPrompt('TextPrompt'))
            .addDialog(new WaterfallDialog('WaterfallDialog', [
                this.promptStep.bind(this),
                this.finalStep.bind(this)
            ]));

        this.initialDialogId = 'WaterfallDialog';
    }

    async promptStep(stepContext) {
        return await stepContext.prompt('TextPrompt', { prompt: MessageFactory.text('Hi there') });
    }

    async finalStep(stepContext) {
        return await stepContext.endDialog();
    }
}

describe('CancelAndHelpDialog', () => {
    describe('Should be able to cancel', () => {
        const testCases = ['cancel', 'quit'];

        testCases.map(testData => {
            it(testData, async () => {
                const sut = new TestCancelAndHelpDialog();
                const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

                // Execute the test case
                let reply = await client.sendActivity('Hi');
                assert.strictEqual(reply.text, 'Hi there');
                assert.strictEqual(client.dialogTurnResult.status, 'waiting');

                reply = await client.sendActivity(testData);
                assert.strictEqual(reply.text, 'Cancelling...');
                assert.strictEqual(client.dialogTurnResult.status, 'cancelled');
            });
        });
    });

    describe('Should be able to get help', () => {
        const testCases = ['help', '?'];

        testCases.map(testData => {
            it(testData, async () => {
                const sut = new TestCancelAndHelpDialog();
                const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

                // Execute the test case
                let reply = await client.sendActivity('Hi');
                assert.strictEqual(reply.text, 'Hi there');
                assert.strictEqual(client.dialogTurnResult.status, 'waiting');

                reply = await client.sendActivity(testData);
                assert.strictEqual(reply.text, 'Show help here');
                assert.strictEqual(client.dialogTurnResult.status, 'waiting');
            });
        });
    });
});
