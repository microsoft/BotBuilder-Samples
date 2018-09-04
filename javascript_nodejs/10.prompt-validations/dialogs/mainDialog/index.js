// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActionTypes, MessageFactory } = require('botbuilder');

const { TextPrompt, NumberPrompt, DatetimePrompt, ChoicePrompt, DialogSet } = require('botbuilder-dialogs');

const USER_NAME_PROP = 'user_name';
const AGE_PROP = 'user_age';
const DOB_PROP = 'user_dob';
const COLOR_PROP = 'user_color';
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

        this.userName = this.userState.createProperty(USER_NAME_PROP)
        this.userAge = this.userState.createProperty(AGE_PROP);
        this.userDob = this.userState.createProperty(DOB_PROP);
        this.userColor = this.userState.createProperty(COLOR_PROP);

        this.dialogs = new DialogSet();
        
        // Add prompts
        // namePrompt will validate that the user's response is between 1 and 50 chars in length
        this.dialogs.add('namePrompt', new TextPrompt(async (context, value) => {
            if (value.length < 1) {
                context.sendActivity('Your name has to include at least one character.');
                return undefined;
            } else if (value.length > 50) {
                context.sendActivity(`Sorry, but I can only handle names of up to 50 characters.  Yours was ${ value.length }.`);
                return undefined;
            } else {
                return value;
            }
        }));

        // agePrompt will validate an age between 1 and 99
        this.dialogs.add('agePrompt', new NumberPrompt(async (context, value) =>{
            if (value < 1 || value > 99) {
                context.sendActivity('Please enter an age in years between 1 and 99');
                return undefined;
            } else {
                return value;
            }
        }));

        // dobPrompt will validate a date between 8/24/1918 and 8/24/2018
        const DATE_LOW_BOUNDS = new Date('8/24/1918');
        const DATE_HIGH_BOUNDS = new Date('8/24/2018');
        this.dialogs.add('dobPrompt', new DatetimePrompt(async (context, values) => {
            try {
                if (!Array.isArray(values) || values.length < 0) { throw new Error('missing time') }
                if ((values[0].type !== 'datetime') && (values[0].type !== 'date')) { throw new Error('unsupported type') }
                const value = new Date(values[0].value);
                if (value.getTime() < DATE_LOW_BOUNDS.getTime()) {
                    throw new Error('too low');
                } else if (value.getTime() > DATE_HIGH_BOUNDS.getTime()) {
                    throw new Error('too high');
                }
                return value;
            } catch (err) {
                await context.sendActivity(`Answer with a date like 8/8/2018 or say "cancel".`);
                return undefined;
            }
        }));
                
        // colorPrompt provides a validation error when a valid choice is not made
        this.dialogs.add('colorPrompt', new ChoicePrompt(async (context, choice) => {
            if (!choice) {
                // an invalid choice was received, emit an error.
                context.sendActivity(`Sorry, "${ context.activity.text }" is not on my list.`);
            } 
            return choice;
        }));

        // Create a dialog flow that captures a series of values from a user
        this.dialogs.add(WHO_ARE_YOU, [
            async function(dc) {
                return await dc.prompt('namePrompt', `What is your name, human?`);
            },
            async function(dc, user_name) {
                await that.userName.set(dc.context, user_name);
                return await dc.prompt('agePrompt', `What is your age?`);
            },
            async function(dc, user_age) {
                await that.userAge.set(dc.context, user_age);
                return await dc.prompt('dobPrompt', `What is your date of birth?`);
            },
            async function(dc, user_dob) {
                await that.userDob.set(dc.context, user_dob);
                return await dc.prompt('colorPrompt', `Finally, what is your favorite color?`, ['red','blue','green']);
            },
            async function(dc, user_color) {
                await that.userColor.set(dc.context, user_color);
                await dc.context.sendActivity(`Your profile is complete! Thank you.`);
                return await dc.end();
            }
        ]);


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(HELLO_USER, [
            async function(dc) {
                const user_name = await that.userName.get(dc.context, null);
                await dc.context.sendActivity(`You asked me to call you ${user_name}.`);
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
            await context.sendActivity('I am a bot that demonstrates the TextPrompt class to collect your name, store it in UserState, and display it. Say anything to continue.');
        }
    }
}

module.exports = MainDialog;