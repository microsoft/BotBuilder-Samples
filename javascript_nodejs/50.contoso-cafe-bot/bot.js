// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory } = require('botbuilder');
const welcomeCard = require('./dialogs/welcome');
const myBotState = require('./botState');
// Bot is reposible for 4 things - 
// 1. Holds user and conversation state property accessors
// 2. Does input processing on incoming activity and updates conversation/ user state. This includes
//      a. handling all activity types
//      b. processing card inputs
//      c. root level NLP for messages
// 3. Hands over execution context to main dialog
// 4. Responsible for handling no-match response from mainDialog when main dialog is unable to continue the conversation
class Bot {
    
    constructor (conversationState, userState, botConfig) {
        if(!conversationState) throw ('Need converstaion state');
        if(!userState) throw ('Need user state');
        if(!botConfig) throw ('Need bot config');
        // Create state objects for user and conversation state properties.
        this.userProfileProperty = new myBotState.UserProfile(userState, USER_PROFILE_PROPERTY);
        this.userReservationsProperty = new myBotState.Reservations(userState, RESERVATIONS_PROPERTY);
        this.userQueryProperty = new myBotState.Query(userState, USER_QUERY_PROPERTY);
        this.intentProperty = new myBotState.Intent(conversationState, USER_INTENT_PROPERTY);
        this.entitiesProperty = new myBotState.Entities(conversationState, ENTITIES_PROPERTY);
    }
    
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about message and other activity types.
        if (context.activity.type === ActivityTypes.Message) {
            // Handle card input (if any) and update state

            // Do we have any oustanding dialogs? if so, continue them and get results

            // Examine results from active dialog for interruption

            // No match from active dialog or no active dialog? 

            // Call root NLP model to get results and update state

            // Dispatch to main dialog
            // Dispatch to right dialog based on root NLP or card input 
        }
        else if(context.activity.type === ActivityTypes.ConversationUpdate && context.activity.membersAdded[0].name !== 'Bot') {
            // Welcome user.
            await context.sendActivity(`Hello, I am the Contoso Cafe Bot!`);
            await context.sendActivity(`I can help book a table, find cafe locations and more..`);
            // Send welcome card.
            await context.sendActivity({ attachments: [CardFactory.adaptiveCard(welcomeCard)]});
        }
    }
}

module.exports = Bot;