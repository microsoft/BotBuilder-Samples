// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
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
            this.askForName.bind(this),
            this.collectAndDisplayName.bind(this)
        ]));


        // Create a dialog that displays a user name after it has been collceted.
        this.dialogs.add(new WaterfallDialog(HELLO_USER, [
            this.displayName.bind(this)
        ]));
    }

    // The first step in this waterfall asks the user for their name.
    async askForName(dc, step) {
        return await dc.prompt(NAME_PROMPT, `What is your name, human?`);
    }

    // The second step in this waterfall collects the response, stores it in
    // the state accessor, then displays it.
    async collectAndDisplayName(step) {
        await this.userName.set(step.context, step.result);
        await step.context.sendActivity(`Got it. You are ${ step.result }.`);
        return await step.end();
    }

    // This step loads the user's name from state and displays it.
    async displayName(step) {
            const user_name = await this.userName.get(step.context, null);
            await step.context.sendActivity(`Your name is ${user_name}.`);
            return await step.end();
    }

    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === 'message') {
            // Create dialog context
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
            
            // Continue the current dialog
            if (!turnContext.responded) {
                await dc.continue();
            }

            // Show menu if no response sent
            if (!turnContext.responded) {
                var user_name = await this.userName.get(dc.context,null);
                if (user_name) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
            
        } else if (
            turnContext.activity.type === ActivityTypes.ConversationUpdate &&
            turnContext.activity.membersAdded[0].name !== 'Bot'
       ) {
           // send a "this is what the bot does" message
            await turnContext.sendActivity('I am a bot that demonstrates the TextPrompt class to collect your name, store it in UserState, and display it. Say anything to continue.');
        }

        // Save changes to the user name.
        await this.userState.saveChanges(turnContext);

        // End this turn by saving changes to the conversation state.
        await this.conversationState.saveChanges(turnContext);

    }

}

module.exports = MainDialog;