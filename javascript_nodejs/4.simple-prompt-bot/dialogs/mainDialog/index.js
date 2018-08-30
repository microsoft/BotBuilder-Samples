// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ActionTypes, MessageFactory
} = require('botbuilder');

const { 
    DialogSet, TextPrompt, ConfirmPrompt, ChoicePrompt, DatetimePrompt, NumberPrompt, 
    AttachmentPrompt
} = require('botbuilder-dialogs');

class MainDialog {
    /**
     * 
     * @param {Object} conversationState 
     */
    constructor (conversationState) {
        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        // this.countProperty = conversationState.createProperty(TURN_COUNTER);
        this.conversationState = conversationState;

        this.dialogs = new DialogSet();
        
        // Add prompts
        this.dialogs.add('choicePrompt', new ChoicePrompt());
        this.dialogs.add('confirmPrompt', new ConfirmPrompt());
        this.dialogs.add('datetimePrompt', new DatetimePrompt());
        this.dialogs.add('numberPrompt', new NumberPrompt());
        this.dialogs.add('textPrompt', new TextPrompt());
        this.dialogs.add('attachmentPrompt', new AttachmentPrompt());
            
        //-----------------------------------------------
        // Main Menu
        //-----------------------------------------------
        this.dialogs.add('mainMenu', [
            async function(dc) {
                function choice(title, value) {
                    return {
                        value: value,
                        action: { type: ActionTypes.ImBack, title: title, value: title }
                    };
                }
                await dc.prompt('choicePrompt', `Select a demo to run:`, [
                    choice('choice', 'choiceDemo'),
                    choice('confirm', 'confirmDemo'),
                    choice('datetime', 'datetimeDemo'),
                    choice('number', 'numberDemo'),
                    choice('text', 'textDemo'),
                    choice('attachment', 'attachmentDemo'),
                    // choice('<all>', 'runAll')
                ]);
            },
            async function(dc, choice) {
                // if (choice.value === 'runAll') {
                    // await dc.replace(choice.value);
                // } else {
                    await dc.context.sendActivity(`The demo will loop so say "menu" or "cancel" to end.`);
                    await dc.replace('loop', { dialogId: choice.value });
                // }
            }
        ]);



        this.dialogs.add('loop', [
            async function(dc, args) {
                dc.activeDialog.state = args;
                await dc.begin(args.dialogId);
            },
            async function(dc) {
                const args = dc.activeDialog.state;
                await dc.replace('loop', args);
            }
        ]);


        //-----------------------------------------------
        // Choice Demo
        //-----------------------------------------------
        this.dialogs.add('choiceDemo', [
            async function(dc) {
                await dc.prompt('choicePrompt', `choice: select a color`, ['red', 'green', 'blue']);
            },
            async function(dc, choice) {
                await dc.context.sendActivity(`Recognized choice: ${JSON.stringify(choice)}`);
                await dc.end();
            }
        ]);


        //-----------------------------------------------
        // Confirm Demo
        //-----------------------------------------------
        this.dialogs.add('confirmDemo', [
            async function(dc) {
                await dc.prompt('confirmPrompt', `confirm: answer "yes" or "no"`);
            },
            async function(dc, value) {
                await dc.context.sendActivity(`Recognized value: ${value}`);
                await dc.end();
            }
        ]);


        //-----------------------------------------------
        // Datetime Demo
        //-----------------------------------------------
        this.dialogs.add('datetimeDemo', [
            async function(dc) {
                await dc.prompt('datetimePrompt', `datetime: enter a datetime`);
            },
            async function(dc, values) {
                await dc.context.sendActivity(`Recognized values: ${JSON.stringify(values)}`);
                await dc.end();
            }
        ]);


        //-----------------------------------------------
        // Number Demo
        //-----------------------------------------------
        this.dialogs.add('numberDemo', [
            async function(dc) {
                await dc.prompt('numberPrompt', `number: enter a number`);
            },
            async function(dc, value) {
                await dc.context.sendActivity(`Recognized value: ${value}`);
                await dc.end();
            }
        ]);


        //-----------------------------------------------
        // Text Demo
        //-----------------------------------------------
        this.dialogs.add('textDemo', [
            async function(dc) {
                await dc.prompt('textPrompt', `text: enter some text`);
            },
            async function(dc, value) {
                await dc.context.sendActivity(`Recognized value: ${value}`);
                await dc.end();
            }
        ]);


        //-----------------------------------------------
        // Attachment Demo
        //-----------------------------------------------
        this.dialogs.add('attachmentDemo', [
            async function(dc) {
                await dc.prompt('attachmentPrompt', `attachment: upload image(s)`);
            },
            async function(dc, values) {
                await dc.context.sendActivity(MessageFactory.carousel(values, `Uploaded ${values.length} Attachment(s)`));
                await dc.end();
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

            // Check for cancel
            const utterance = (context.activity.text || '').trim().toLowerCase();
            if (utterance === 'menu' || utterance === 'cancel') { 
                await dc.endAll(); 
            }
            
            // Continue the current dialog
            await dc.continue();

            // Show menu if no response sent
            if (!context.responded) {
                await dc.begin('mainMenu');
            }
        } else if (context.activity.type == 'conversationUpdate' && context.activity.membersAdded[0].id === 'default-user') {
            // Create dialog context
            const state = this.conversationState.get(context);
            const dc = this.dialogs.createContext(context, state);

            await dc.begin('mainMenu');
        }
    }
}

module.exports = MainDialog;