// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');

const DIALOG_STATE_PROPERTY = 'dialogState';
const USER_NAME_PROP = 'user_name';
const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

const NAME_PROMPT = 'name_prompt';


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

        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        this.userName = this.userState.createProperty(USER_NAME_PROP)

        this.dialogs = new DialogSet(this.dialogState);
        
        // Add prompts
        this.dialogs.add(new TextPrompt(NAME_PROMPT));
            
        // Create a dialog that asks the user for their name.
        this.dialogs.add(new WaterfallDialog(WHO_ARE_YOU, [
            async (dc, step) => {
                return await dc.prompt(NAME_PROMPT, `What is your name, human?`);
            },
            async (dc, step) => {
                await this.userName.set(dc.context, step.result);
                await dc.context.sendActivity(`Got it. You are ${ step.result }`);
                return await dc.end();
            }
        ]));


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(new WaterfallDialog(HELLO_USER, [
            async (dc, step) => {
                const user_name = await this.userName.get(dc.context, null);
                await dc.context.sendActivity(`Your name is ${user_name}.`);
                return await dc.end();
            }
        ]));
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
                if (dc.activeDialog) {
                    await dc.cancelAll();
                    await dc.context.sendActivity(`Ok... Cancelled.`);
                } else {
                    await dc.context.sendActivity(`Nothing to cancel.`);
                }
            }
            
            // Continue the current dialog
            if (!context.responded) {
                await dc.continue();
            }

            // Show menu if no response sent
            if (!context.responded) {
                var user_name = await this.userName.get(dc.context,null);
                if (user_name) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].name !== 'Bot') {
            // send a "this is what the bot does" message
            await context.sendActivity('I am a bot that demonstrates the TextPrompt class to collect your name, store it in UserState, and display it. Say anything to continue.');
        }
    }
}

module.exports = MainDialog;