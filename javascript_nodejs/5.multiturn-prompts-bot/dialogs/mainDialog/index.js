// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { ChoicePrompt, DialogSet, NumberPrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const DIALOG_STATE_PROPERTY = 'dialogState';
const USER_PROFILE_PROPERTY = 'user';

const WHO_ARE_YOU = 'who_are_you';
const HELLO_USER = 'hello_user';

const NAME_PROMPT = 'name_prompt';
const CONFIRM_PROMPT = 'confirm_prompt';
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
     
        // Add prompts that will be used by the main dialogs.
        this.dialogs.add(new TextPrompt(NAME_PROMPT));
        this.dialogs.add(new ChoicePrompt(CONFIRM_PROMPT));
        this.dialogs.add(new NumberPrompt(AGE_PROMPT, async (turnContext, step)=> {
            if (step.recognized.value < 0) {
                await turnContext.sendActivity(`Your age can't be less than zero.`);
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
                const user = await this.userProfile.get(dc.context, {});
                user.name = step.result;
                await this.userProfile.set(dc.context, user);
                await dc.prompt(CONFIRM_PROMPT, 'Do you want to give your age?', ['yes','no']);                
            },
            async (dc, step) => {
                if (step.result && step.result.value === 'yes') {
                    return await dc.prompt(AGE_PROMPT,`What is your age?`,
                        {
                            retryPrompt: 'Sorry, please specify your age as a positive number or say cancel.'
                        }
                    );
                } else {
                    return await step.next(-1);
                }
            },
            async (dc, step) => {
                const user = await this.userProfile.get(dc.context, {});
                if (step.result !== -1) {
                    user.age = step.result;
                    await this.userProfile.set(dc.context, user);
                    await dc.context.sendActivity(`I will remember that you are ${ step.result } years old.`);
                } else {
                    await dc.context.sendActivity(`No age given.`);
                }
                return await dc.end();
            }
        ]));

        // Create a dialog that displays a user name after it has been collected.
        this.dialogs.add(new WaterfallDialog(HELLO_USER, [
            async (dc) => {
                const user = await this.userProfile.get(dc.context, {});
                if (user.age) {
                    await dc.context.sendActivity(`Your name is ${ user.name } and you are ${ user.age } years old.`);
                } else {
                    await dc.context.sendActivity(`Your name is ${ user.name } and you did not share your age.`);
                }
                return await dc.end();
            }
        ]));
    }

    /**
     * 
     * @param {TurnContext} turnContext A TurnContext object that will be interpreted and acted upon by the bot.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === 'message') {
            // Create a dialog context object.
            const dc = await this.dialogs.createContext(turnContext);

            const utterance = (turnContext.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                if (dc.activeDialog) {
                    await dc.cancelAll();
                    await dc.context.sendActivity(`Ok... canceled.`);
                } else {
                    await dc.context.sendActivity(`Nothing to cancel.`);
                }
            }
            
            // If the bot has not yet responded, continue processing the current dialog.
            await dc.continue();

            // Start the sample dialog in response to any other input.
            if (!turnContext.responded) {
                const user = await this.userProfile.get(dc.context, {});
                if (user.name) {
                    await dc.begin(HELLO_USER)
                } else {
                    await dc.begin(WHO_ARE_YOU)
                }
            }
        } else if (
            turnContext.activity.type === ActivityTypes.ConversationUpdate &&
            turnContext.activity.membersAdded[0].name !== 'Bot'
       ) {
           // Send a "this is what the bot does" message.
            const description = [
                'I am a bot that demonstrates the TextPrompt and NumberPrompt classes',
                'to collect your name and age, then store those values in UserState for later use.',
                'Say anything to continue.'
            ];
            await turnContext.sendActivity(description.join(' '));
        }

        // Save changes to the user state.
        this.userState.write(turnContext);

        // End this turn by saving changes to the conversation state.
        this.conversationState.write(turnContext);

    }
}

module.exports = MainDialog;