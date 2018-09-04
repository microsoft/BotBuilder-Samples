// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, NumberPrompt, DateTimePrompt, ChoicePrompt, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');

const OnboardingDialog = require('../onboard');

const DIALOG_STATE_PROP = 'dialogState';

const ONBOARD_USER = 'onboard_user';


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
        
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROP);

        this.dialogs = new DialogSet(this.dialogState);

        this.dialogs.add(new OnboardingDialog(ONBOARD_USER, this.userState));
    }


    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {
            // Create dialog context
            const state = this.conversationState.get(context);
            const dc = await this.dialogs.createContext(context, state);

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