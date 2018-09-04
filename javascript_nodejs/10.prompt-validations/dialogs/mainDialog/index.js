// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogSet } = require('botbuilder-dialogs');

const OnboardingDialog = require('../onboard');

const DIALOG_STATE_PROP = 'dialogState';

const ONBOARD_USER = 'onboard_user';

const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const DOB_PROP = 'user_dob';
const COLOR_PROP = 'user_color';

class MainDialog {
    /**
     * 
     * @param {Object} conversationState 
     * @param {Object} userState 
     */
    constructor (conversationState, userState) {

        // creates a new state accessor property. see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.conversationState = conversationState;
        this.userState = userState;
        
        // create a property used to store dialog state
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROP);

        // create a dialog set to include our dialogs
        this.dialogs = new DialogSet(this.dialogState);

        // create some properties used to store these values
        this.userName = this.userState.createProperty(USER_NAME_PROP)
        this.userAge = this.userState.createProperty(AGE_PROP);
        this.userDob = this.userState.createProperty(DOB_PROP);
        this.userColor = this.userState.createProperty(COLOR_PROP);

        // create the main user onboarding dialog
        this.dialogs.add(new OnboardingDialog(ONBOARD_USER, this.userName, this.userAge, this.userDob, this.userColor));
    }


    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {

            // Create dialog context
            const dc = await this.dialogs.createContext(context);

            const utterance = (context.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                await dc.cancelAll(); 
            }
            
            // Continue the current dialog if one is pending
            if (!context.responded) {
                await dc.continue();
            }

            // Show menu if no response sent
            if (!context.responded) {
                await dc.begin(ONBOARD_USER)
            } 
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].id === 'default-user') {
            // send a "this is what the bot does" message
            await context.sendActivity('I am a bot that demonstrates the TextPrompt class to collect your name, store it in UserState, and display it. Say anything to continue.');
        }
    }
}

module.exports = MainDialog;