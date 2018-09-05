// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogSet } = require('botbuilder-dialogs');

const OnboardingDialog = require('../onboard');

const DIALOG_STATE_PROPERTY = 'dialogState';

const USER_NAME_PROPERTY = 'user_name';
const AGE_PROPERTY = 'user_age';
const DOB_PROPERTY = 'user_dob';
const COLOR_PROPERTY = 'user_color';

const ONBOARD_USER = 'onboard_user';

class MainDialog {
    /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store dialog state.
     * @param {UserState} userState A UserState object used to store user profile information.
     */
    constructor (conversationState, userState) {

        // Creates a new state accessor property. 
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.conversationState = conversationState;
        this.userState = userState;
        
        // Create a property used to store dialog state.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create a dialog set to include the dialogs used by this bot.
        this.dialogs = new DialogSet(this.dialogState);

        // Create some properties used to store values from the user.
        this.userName = this.userState.createProperty(USER_NAME_PROPERTY)
        this.userAge = this.userState.createProperty(AGE_PROPERTY);
        this.userDob = this.userState.createProperty(DOB_PROPERTY);
        this.userColor = this.userState.createProperty(COLOR_PROPERTY);

        // Create the main user onboarding dialog.
        this.dialogs.add(new OnboardingDialog(ONBOARD_USER, this.userName, this.userAge, this.userDob, this.userColor));
    }


    /**
     * 
     * @param {TurnContext} context A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {

            // Create dialog context
            const dc = await this.dialogs.createContext(context);

            const utterance = (context.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                if (dc.activeDialog) {
                    await dc.cancelAll();
                    await dc.context.sendActivity(`Ok... Cancelled.`);
                } else {
                    await dc.context.sendActivity(`Nothing to cancel.`);
                }
            }
            
            // Continue the current dialog if one is pending.
            if (!context.responded) {
                await dc.continue();
            }

            // If no response has been sent, start the onboarding dialog.
            if (!context.responded) {
                await dc.begin(ONBOARD_USER)
            } 
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].id === 'default-user') {
            // send a "this is what the bot does" message
            const description = [
                'I am a bot that demonstrates the TextPrompt class to collect your name,',
                'store it in UserState, and display it.',
                'Say anything to continue.'
            ];
            await context.sendActivity(description.join(' '));
        }
    }
}

module.exports = MainDialog;