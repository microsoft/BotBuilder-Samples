// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


const { ActivityTypes, MessageFactory } = require('botbuilder');
const { ChoicePrompt, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');
const connectionName = "";
const DIALOG_STATE_PROPERTY = 'dialogState';
const OAUTH_PROMPT = 'oAuth_prompt';
const CONFIRM_PROMPT = 'confirm_prompt';
const AUTH_DIALOG = 'auth_dialog';
const HELP_TEXT = 'This bot will introduce you to Authentication.' +
                   ' Type anything to get logged in. Type \'logout\' to signout.' +
                   ' Type \'help\' to view this message again';
/**
 * A simple bot that authenticates users using OAuth prompts.
 */
class AuthenticationBot {

     /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     */
    constructor (conversationState){

        // Create a new state accessor property. See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.conversationState = conversationState;
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.dialogs = new DialogSet(this.dialogState);       
        // Add prompts that will be used by the bot.
        //this.dialogs.add(new OAuthPrompt(NAME_PROMPT));
        this.dialogs.add(new ChoicePrompt(CONFIRM_PROMPT));
        this.dialogs.add(new WaterfallDialog(AUTH_DIALOG,[

        ]));
    }

    /**
     * Every conversation turn for our AuthenticationBot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {Object} turnContext on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;

            // Create an array with the valid color options.
            const validColors = ['logout', 'help'];

            // If the `text` is in the Array, a valid color was selected and send agreement. 
            if (validColors.includes(text)) {
                if (text === 'help'){
                    await turnContext.sendActivity(HELP_TEXT); 
                }
                
            } else {
                await turnContext.sendActivity('Please select a color.');
            }

            // After the bot has responded send the SuggestedActions.
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            let members = turnContext.activity.membersAdded;

            for (let index = 0; index < members.length; index++) {
                const member = members[index];
                if (member.id != turnContext.activity.recipient.id) {
                    const welcomeMessage = `Welcome to AuthenticationBot ${member.name}. ` + HELP_TEXT;                         
                    await turnContext.sendActivity(welcomeMessage);
                }
            };
        } else {
            await turnContext.sendActivity(`[${turnContext.activity.type} event detected.]`);
        }
    }
}

module.exports = AuthenticationBot;