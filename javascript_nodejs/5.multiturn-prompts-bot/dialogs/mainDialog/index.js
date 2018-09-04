// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, NumberPrompt, DialogSet, WaterfallDialog } = require('botbuilder-dialogs');

const DIALOG_STATE_PROPERTY = 'dialogState';
const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

const NAME_PROMPT = 'name_prompt';
const AGE_PROMPT = 'age_prompt';

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

        this.userName = this.userState.createProperty(USER_NAME_PROP);
        this.userAge = this.userState.createProperty(AGE_PROP);

        this.dialogs = new DialogSet(this.dialogState);
     
        // Add prompts
        this.dialogs.add(new TextPrompt(NAME_PROMPT));
        this.dialogs.add(new NumberPrompt(AGE_PROMPT, async (context, step)=> {
            if (step.recognized.value < 0) {
                await context.sendActivity(`Your age can't be less than zero.`);
            } else {
                step.end(step.recognized.value);
            }
        }));
            
        // Create a dialog that asks the user for their name.
        this.dialogs.add(new WaterfallDialog(WHO_ARE_YOU,[
            async (dc) => {
                return await dc.prompt(NAME_PROMPT, `What is your name, human?`);
            },
            async (dc, step) => {
                await this.userName.set(dc.context, step.result);
                return await dc.prompt(AGE_PROMPT,`And what is your age, ${ step.result }?`,
                    {
                        retryPrompt: 'Sorry, please specify your age as a positive number or say cancel.'
                    }
                );
            },
            async (dc, step) => {
                await this.userAge.set(dc.context, step.result);
                await dc.context.sendActivity(`I will remember that you are ${ step.result } years old.`);
                return await dc.end();
            }
        ]));


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(new WaterfallDialog(HELLO_USER, [
            async (dc) => {
                const user_name = await this.userName.get(dc.context, null);
                const user_age = await this.userAge.get(dc.context, null);
                await dc.context.sendActivity(`Your name is ${ user_name } and you are ${ user_age } years old.`);
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
               return await dc.cancelAll(); 
            }
            
            // Continue the current dialog
            if (!context.responded) {
                await dc.continue();
            }

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