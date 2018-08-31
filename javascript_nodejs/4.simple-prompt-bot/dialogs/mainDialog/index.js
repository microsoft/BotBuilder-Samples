// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, DialogSet } = require('botbuilder-dialogs');

const USER_NAME_PROP = 'user_name';
const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

class MainDialog {
    /**
     * 
     * @param {Object} conversationState 
     */
    constructor (conversationState, userState) {
        var that = this;

        // creates a new state accessor property. see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.conversationState = conversationState;
        this.userState = userState;

        this.userName = this.userState.createProperty(USER_NAME_PROP)

        this.dialogs = new DialogSet();
        
        // Add prompts
        this.dialogs.add('textPrompt', new TextPrompt());
            
        // Create a dialog that asks the user for their name.
        this.dialogs.add(WHO_ARE_YOU, [
            async function(dc) {
                return await dc.prompt('textPrompt', `What is your name, human?`);
            },
            async function(dc, value) {
                const user_name = await that.userName.set(dc.context, value);
                await dc.context.sendActivity(`Got it. You are ${value}`);
                return await dc.end();
            }
        ]);


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(HELLO_USER, [
            async function(dc) {
                const user_name = await that.userName.get(dc.context, null);
                await dc.context.sendActivity(`Your name is ${user_name}.`);
                return await dc.end();
            }
        ]);
    }


    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {
        // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
        if (context.activity.type === 'message') {
            // Create dialog context
            const state = this.conversationState.get(context);
            const dc = this.dialogs.createContext(context, state);

            const utterance = (context.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                await dc.endAll(); 
            }
            
            // Continue the current dialog
            await dc.continue();

            // Show menu if no response sent
            if (!context.responded) {
                var user_name = await this.userName.get(dc.context,null);
                if (user_name) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].id === 'default-user') {
            // send a "this is what the bot does" message
        }
    }
}

module.exports = MainDialog;