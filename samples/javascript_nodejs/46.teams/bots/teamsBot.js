// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogBot } = require('./dialogBot');

class TeamsBot extends DialogBot {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     * @param {Dialog} dialog
     * @param {any} logger object for logging events, defaults to console if none is provided
     */
    constructor(conversationState, userState, dialog, logger) {
        super(conversationState, userState, dialog, logger);

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Welcome to AuthenticationBot. Type anything to get logged in. Type \'logout\' to sign-out.');
                }
            }

            await next();
        });

        this.onInvoke(async (context, next) => {
            console.log('Running dialog with Invoke Activity.');
            await this.dialog.run(context, this.dialogState);

            await next();
        });

        this.onReactionsAdded(async (context, next) => {
            const reactionsAdded = context.activity.reactionsAdded;
            for (let cnt = 0; cnt < reactionsAdded.length; cnt++) {
                await context.sendActivity(`add: ${ reactionsAdded[cnt].type }`);
            }

            await next();
        });

        this.onReactionsRemoved(async (context, next) => {
            const reactionsRemoved = context.activity.reactionsRemoved;
            for (let cnt = 0; cnt < reactionsRemoved.length; cnt++) {
                await context.sendActivity(`remove: ${ reactionsRemoved[cnt].type }`);
            }

            await next();
        });
    }
}

module.exports.TeamsBot = TeamsBot;
