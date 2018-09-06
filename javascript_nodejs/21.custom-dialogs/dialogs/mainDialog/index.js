// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TextPrompt, DialogSet } = require('botbuilder-dialogs');

const ScriptedDialog = require('./scripted.js');

const DIALOG_STATE_PROPERTY = 'dialogState';
const USER_RESPONSES = 'user';

const CHECKIN_DIALOG = 'checkin';

class MainDialog {
    /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     * @param {UserState} userState A UserState object used to store values specific to the user.
     */
    constructor (conversationState, userState) {

        // Create a new state accessor property. See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.conversationState = conversationState;
        this.userState = userState;

        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        this.userResponses = this.userState.createProperty(USER_RESPONSES);

        this.dialogs = new DialogSet(this.dialogState);

        this.dialogs.add(new ScriptedDialog(CHECKIN_DIALOG, __dirname + '/resources/checkin.json', async (dc, results) => {
            console.log('Checkin completed: ', results);
            await this.userResponses.set(dc.context, results);
        }));

        this.dialogs.add(new TextPrompt('TextPrompt'));

    }


    /**
     * 
     * @param {TurnContext} context A TurnContext object that will be interpretted and acted upon by the bot.
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
            
            // If the bot has not yet responded, continue processing the current dialog.
            if (!context.responded) {
                await dc.continue();
            }

            // Start the sample dialog in response to any other input.
            if (!context.responded) {
                await dc.begin(CHECKIN_DIALOG);
            }
        } else if (context.activity.type === 'conversationUpdate' && context.activity.membersAdded[0].name !== 'Bot') {
            // Send a "this is what the bot does" message.
            const description = [
                'Say anything to continue.'
            ];
            await context.sendActivity(description.join(' '));
        }
    }
}

module.exports = MainDialog;