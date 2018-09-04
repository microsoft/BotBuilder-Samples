// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, NumberPrompt, DialogSet } = require('botbuilder-dialogs');

const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

class MainDialog {
    /**
     * 
     * @param {Object} conversationState 
     * @param {Object} userState 
     */
    constructor (conversationState, userState) {
        var that = this;

        // creates a new state accessor property. see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.conversationState = conversationState;
        this.userState = userState;

        this.userName = this.userState.createProperty(USER_NAME_PROP);
        this.userAge = this.userState.createProperty(AGE_PROP);

        this.dialogs = new DialogSet();
        
        // Add prompts
        this.dialogs.add('textPrompt', new TextPrompt());
        this.dialogs.add('numberPrompt', new NumberPrompt(async (context, value)=> {
            if (value < 0) {
                await context.sendActivity(`Your age can't be less than zero.`);
                return undefined;
            } else {
                return value;
            }
        }));
            
        // Create a dialog that asks the user for their name.
        this.dialogs.add(WHO_ARE_YOU, [
            async function(dc) {
                return await dc.prompt('textPrompt', `What is your name, human?`);
            },
            async function(dc, name_value) {
                await that.userName.set(dc.context, name_value);
                return await dc.prompt('numberPrompt',`And what is your age, ${ name_value }?`,
                    {
                        retryPrompt: 'Sorry, please specify your age as a positive number or say cancel.'
                    }
                );
            },
            async function(dc, age_value) {
                await that.userAge.set(dc.context, age_value);
                await dc.context.sendActivity(`I will remember that you are ${ age_value } years old.`);
                return await dc.end();
            }
        ]);


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(HELLO_USER, [
            async function(dc) {
                const user_name = await that.userName.get(dc.context, null);
                const user_age = await that.userAge.get(dc.context, null);
                await dc.context.sendActivity(`Your name is ${ user_name } and you are ${ user_age } years old.`);
                return await dc.end();
            }
        ]);
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
            const dc = this.dialogs.createContext(context, state);

            const utterance = (context.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
               return await dc.endAll(); 
            }
            
            // Continue the current dialog
            await dc.continue();

            // Show menu if no response sent
            if (!context.responded) {
                var user_name = await this.userName.get(dc.context,null);
                var user_age = await this.userAge.get(dc.context,null);
                if (user_name && user_age) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].id === 'default-user') {
            // send a "this is what the bot does" message
            await context.sendActivity('I am a bot that demonstrates the TextPrompt and NumberPrompt classes to collect your name and age, then store those values in UserState for later use. Say anything to continue.');
        }
    }
}

module.exports = MainDialog;