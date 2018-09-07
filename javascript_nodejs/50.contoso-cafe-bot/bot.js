// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory } = require('botbuilder');
const welcomeCard = require('./dialogs/mainDialog/resources/welcomeCard.json');
class Bot {
    /**
     * 
     * @param {Object} conversationState 
     */
    constructor (conversationState) {
        
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
        if (context.activity.type === ActivityTypes.Message) {
            
        }
        else if(context.activity.type === ActivityTypes.ConversationUpdate && context.activity.membersAdded[0].name !== 'Bot') {
            // welcome user
            await context.sendActivity(`Hello, I am the Contoso Cafe Bot!`);
            await context.sendActivity(`I can help book a table, find cafe locations and more..`);
            await context.sendActivity({ attachments: [CardFactory.adaptiveCard(welcomeCard)]});
        }
    }
}

module.exports = Bot;