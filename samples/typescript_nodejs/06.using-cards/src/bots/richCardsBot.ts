// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConversationState, MessageFactory, UserState } from 'botbuilder';
import { DialogBot } from './dialogBot';

import { MainDialog } from '../dialogs/mainDialog';
/**
 * RichCardsBot prompts a user to select a Rich Card and then returns the card
 * that matches the user's selection.
 */
export class RichCardsBot extends DialogBot {
    constructor(conversationState: ConversationState, userState: UserState, dialog: MainDialog) {
        super(conversationState, userState, dialog);

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (const memberAdded of membersAdded) {
                if (memberAdded.id !== context.activity.recipient.id) {
                    const reply = MessageFactory.text('Welcome to CardBot. ' +
                        'This bot will show you different types of Rich Cards. ' +
                        'Please type anything to get started.');
                    await context.sendActivity(reply);
                }
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}
