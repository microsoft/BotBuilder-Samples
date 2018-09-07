// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder-core');
const { DialogSet } = require('botbuilder-dialogs');

const OnboardingDialog = require('../onboard');

const DIALOG_STATE_PROPERTY = 'dialogState';

const USER_PROFILE_PROPERTY = 'user';

const ONBOARD_USER = 'onboard_user';

class MainDialog {
    /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store dialog state.
     * @param {UserState} userState A UserState object used to store user profile information.
     */
    constructor (conversationState, userState) {

        this.conversationState = conversationState;
        this.userState = userState;

        // Create a property used to store dialog state.
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create some properties used to store values from the user.
        this.userProfile = this.userState.createProperty(USER_PROFILE_PROPERTY)

        // Create a dialog set to include the dialogs used by this bot.
        this.dialogs = new DialogSet(this.dialogState);

        // Create the main user onboarding dialog.
        this.dialogs.add(new OnboardingDialog(ONBOARD_USER, this.userProfile));
    }


    /**
     * 
     * @param {TurnContext} turnContext A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {

            // Create dialog context.
            const dc = await this.dialogs.createContext(turnContext);

            const utterance = (turnContext.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                if (dc.activeDialog) {
                    await dc.cancelAll();
                    await dc.context.sendActivity(`Ok... Cancelled.`);
                } else {
                    await dc.context.sendActivity(`Nothing to cancel.`);
                }
            }
            
            // Continue the current dialog if one is pending.
            await dc.continue();

            // If no response has been sent, start the onboarding dialog.
            if (!turnContext.responded) {
                await dc.begin(ONBOARD_USER);
            } 
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate && turnContext.activity.membersAdded[0].name !== 'Bot') {
            // Send a "this is what the bot does" message.
            const description = [
                'I am a bot that demonstrates the TextPrompt class to collect your name,',
                'store it in UserState, and display it.',
                'Say anything to continue.'
            ];
            await turnContext.sendActivity(description.join(' '));
        }
    }
}

module.exports = MainDialog;