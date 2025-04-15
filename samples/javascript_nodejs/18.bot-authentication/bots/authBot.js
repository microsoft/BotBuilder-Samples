// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const { DialogBot } = require('./dialogBot');

/** @import { ConversationState, UserState } from 'botbuilder' */
/** @import { MainDialog } from '../dialogs/mainDialog' */

class AuthBot extends DialogBot {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     * @param {MainDialog} dialog
     */
    constructor(conversationState, userState, dialog) {
        super(conversationState, userState, dialog);

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded ?? [];
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Welcome to AuthenticationBot. Type anything to get logged in. Type \'logout\' to sign-out.');
                }
            }

            await next();
        });

        this.onTokenResponseEvent(async (context, next) => {
            console.log('Running dialog with Token Response Event Activity.');
            await this.dialog.run(context, this.dialogState);

            await next();
        });
    }
}

module.exports.AuthBot = AuthBot;
