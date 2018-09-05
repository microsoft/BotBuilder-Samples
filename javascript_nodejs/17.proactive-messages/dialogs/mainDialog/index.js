// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogSet } = require('botbuilder-dialogs');

const CreateReminderDialog = require('../createReminder');

const DIALOG_STATE_PROPERTY = 'dialogState';

const CREATE_REMINDER = 'createreminder';
const INSTRUCTIONS = 'instructions';

class MainDialog {
    /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store dialog state.
     * @param {UserState} userState A UserState object used to store user profile information.
     */
    constructor (conversationState, userState, adapter) {

        // Creates a new state accessor property. 
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.conversationState = conversationState;
        this.userState = userState;
        
        // Create a property used to store dialog state.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create a dialog set to include the dialogs used by this bot.
        this.dialogs = new DialogSet(this.dialogState);

        this.dialogs.add(new CreateReminderDialog(CREATE_REMINDER, adapter));
        
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
                if (utterance.toLowerCase().includes('remind')) {
                    await dc.begin(CREATE_REMINDER);
                } else {
                    await dc.begin(INSTRUCTIONS);
                }
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