// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogBot } = require('./dialogBot');

class AuthBot extends DialogBot {
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

        this.onTokenResponseEvent(async (context, next) => {
            this.logger.log('Running dialog with Token Response Event Activity.');
            await this.dialog.run(context, this.dialogState);

            await next();
        });
    }
}

module.exports.AuthBot = AuthBot;
