// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, NumberPrompt, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');

const ScriptedDialog = require('./scripted.js');

const DIALOG_STATE_PROPERTY = 'dialogState';
const USER_PROFILE_PROPERTY = 'user';

const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

const NAME_PROMPT = 'name_prompt';
const AGE_PROMPT = 'age_prompt';

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

        this.userProfile = this.userState.createProperty(USER_PROFILE_PROPERTY);

        this.dialogs = new DialogSet(this.dialogState);

        this.dialogs.add(new ScriptedDialog(WHO_ARE_YOU, __dirname + '/sample.json', async (results) => {
            console.log('SAMPLE DIALOG COMPLETED WITH RESULTS', results);
        }));

        this.dialogs.add(new ScriptedDialog('followup', __dirname + '/followup.json', async (results) => {
            console.log('FOLLOW UP DIALOG COMPLETED WITH RESULTS', results);
        }));

        this.dialogs.add(new ScriptedDialog('preamble', __dirname + '/preamble.json'));


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
                const user = await this.userProfile.get(dc.context, {});
                if (user.name && user.age) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
        } else if (context.activity.type === 'conversationUpdate' && context.activity.membersAdded[0].name !== 'Bot') {
            // Send a "this is what the bot does" message.
            const description = [
                'I am a bot that demonstrates the TextPrompt and NumberPrompt classes',
                'to collect your name and age, then store those values in UserState for later use.',
                'Say anything to continue.'
            ];
            await context.sendActivity(description.join(' '));
        }
    }
}

module.exports = MainDialog;