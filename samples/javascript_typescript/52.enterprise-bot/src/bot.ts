// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityTypes, ConversationState, UserState, TurnContext } from 'botbuilder';
import { BotConfiguration } from 'botframework-config';

/**
 * Demonstrates the following concepts:
 * 
 */
export class Bot {
    /**
     * Constructs the three pieces necessary for this bot to operate:
     *
     * @param {ConversationState} conversationState property accessor
     * @param {UserState} userState property accessor
     * @param {BotConfiguration} botConfig contents of the .bot file
     */
    constructor(conversationState: ConversationState, userState: UserState, botConfig: BotConfiguration) {
        if (!conversationState) throw ('Missing parameter.  conversationState is required');
        if (!userState) throw ('Missing parameter.  userState is required');
        if (!botConfig) throw ('Missing parameter.  botConfig is required');
    }

    /**
     * Driver code that does one of the following:
     * 1. Nothing
     * @param {Context} context turn context from the adapter
     */
    public onTurn = async (context: TurnContext) => {
        if (context.activity.type === ActivityTypes.Message) {
            await context.sendActivity(`I didn't understand what you just said to me.`);
        } else if (context.activity.type === 'conversationUpdate' && this.getNewMemberName(context) === 'Bot') {
            await context.sendActivity(`Welcome!!`);
        }
    }

    private getNewMemberName(context: TurnContext): string {
        if (context.activity.membersAdded) {
            return context.activity.membersAdded[0].name;
        }

        return '';
    }
};