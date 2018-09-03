// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// User state property
const BOT_USER_STATE = 'botUserState';
// Conversation state property
const BOT_CONVERSATION_STATE = 'botConvoState';

class MainDialog {
    /**
     * 
     * @param {Object} conversation state 
     * @param {Object} user state
     * @param {Object} bot configuration
     */
    constructor (conversationState, userState, botConfig) {
        if(!conversationState || !conversationState.createProperty) throw ('Invalid conversation state provided.');
        if(!userState || !userState.createProperty) throw ('Invalid user state provided.');
        if(!botConfig) throw ('Need bot config');

        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.userStateProperty = userState.createProperty(BOT_USER_STATE);
        this.conversationStateProperty = conversationState.createProperty(BOT_CONVERSATION_STATE);
    }
    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {
        // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
        if (context.activity.type === 'message') {
            // read from state.
            let count = await this.countProperty.get(context);
            count = count === undefined ? 1 : count;
            await context.sendActivity(`${count}: You said "${context.activity.text}"`);
            // increment and set turn counter.
            this.countProperty.set(context, ++count);
        }
        else {
            await context.sendActivity(`[${context.activity.type} event detected]`);
        }
    }
}

module.exports = MainDialog;