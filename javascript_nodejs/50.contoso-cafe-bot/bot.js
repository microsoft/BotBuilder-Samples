// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class Bot {
    /**
     * 
     * @param {Object} conversationState 
     */
    constructor (conversationState) {
        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.countProperty = conversationState.createProperty(TURN_COUNTER);
    }
    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {

        // Bot class is reposible for 4 things - 
        // 1. Keeps hold of user and conversation state property accessors
        // 2. Does input processing on incoming activity and updates conversation/ user state. This includes
        //      a. handling all activity types
        //      b. processing card inputs
        //      c. root level NLP for messages
        // 3. Hands over execution context to main dialog
        // 4. Responsible for handling no-match response from mainDialog when main dialog is unable to continue the conversation

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

module.exports = Bot;