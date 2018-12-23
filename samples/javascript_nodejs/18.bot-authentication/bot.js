// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { ChoicePrompt, DialogSet, OAuthPrompt, WaterfallDialog } = require('botbuilder-dialogs');

// Name of the dialog state used in the constructor.
const DIALOG_STATE_PROPERTY = 'dialogState';

// Names of the prompts the bot uses.
const OAUTH_PROMPT = 'oAuth_prompt';
const CONFIRM_PROMPT = 'confirm_prompt';

// Name of the WaterfallDialog the bot uses.
const AUTH_DIALOG = 'auth_dialog';

// Text to help guide the user through using the bot.
const HELP_TEXT = ' Type anything to get logged in. Type \'logout\' to signout.' +
    ' Type \'help\' to view this message again';

// The connection name here must match the one from
// your Bot Channels Registration on the settings blade in Azure.
const CONNECTION_NAME = '';

// Create the settings for the OAuthPrompt.
const OAUTH_SETTINGS = {
    connectionName: CONNECTION_NAME,
    title: 'Sign In',
    text: 'Please Sign In',
    timeout: 300000 // User has 5 minutes to log in.
};

/**
 * A bot that authenticates users using OAuth prompts.
 */
class AuthenticationBot {
    /**
     * The constructor for the bot.
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     */
    constructor(conversationState) {
        this.conversationState = conversationState;

        // Create a new state accessor property.
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.dialogs = new DialogSet(this.dialogState);

        // Add prompts that will be used by the bot.
        this.dialogs.add(new ChoicePrompt(CONFIRM_PROMPT));
        this.dialogs.add(new OAuthPrompt(OAUTH_PROMPT, OAUTH_SETTINGS));

        // The WaterfallDialog that controls the flow of the conversation.
        this.dialogs.add(new WaterfallDialog(AUTH_DIALOG, [
            this.oauthPrompt,
            this.loginResults,
            this.displayToken
        ]));
    }

    /**
     * Waterfall step that prompts the user to login if they have not already or their token has expired.
     * @param {WaterfallStepContext} step
     */
    async oauthPrompt(step) {
        return await step.prompt(OAUTH_PROMPT);
    }

    /**
     * Waterfall step that informs the user that they are logged in and asks
     * the user if they would like to see their token via a prompt
     * @param {WaterfallStepContext} step
     */
    async loginResults(step) {
        let tokenResponse = step.result;
        if (tokenResponse != null) {
            await step.context.sendActivity('You are now logged in.');
            return await step.prompt(CONFIRM_PROMPT, 'Do you want to view your token?', ['yes', 'no']);
        }

        // Something went wrong, inform the user they were not logged in
        await step.context.sendActivity('Login was not sucessful please try again');
        return await step.endDialog();
    }

    /**
     *
     * Waterfall step that will display the user's token. If the user's token is expired
     * or they are not logged in this will prompt them to log in first.
     * @param {WaterfallStepContext} step
     */
    async displayToken(step) {
        const result = step.result.value;
        if (result === 'yes') {
            // Call the prompt again because we need the token. The reasons for this are:
            // 1. If the user is already logged in we do not need to store the token locally in the bot and worry
            // about refreshing it. We can always just call the prompt again to get the token.
            // 2. We never know how long it will take a user to respond. By the time the
            // user responds the token may have expired. The user would then be prompted to login again.
            //
            // There is no reason to store the token locally in the bot because we can always just call
            // the OAuth prompt to get the token or get a new token if needed.
            let prompt = await step.prompt(OAUTH_PROMPT);
            var tokenResponse = prompt.result;
            if (tokenResponse != null) {
                await step.context.sendActivity(`Here is your token: ${ tokenResponse.token }`);
                await step.context.sendActivity(HELP_TEXT);
                return await step.endDialog();
            }
        }

        await step.context.sendActivity(HELP_TEXT);
        return await step.endDialog();
    }

    /**
     * Every conversation turn for our AuthenticationBot will call this method.
     * @param {TurnContext} turnContext on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Create a dialog context object.
            const dc = await this.dialogs.createContext(turnContext);
            const text = turnContext.activity.text;

            // Create an array with the valid options.
            const validCommands = ['logout', 'help'];
            await dc.continueDialog();

            // If the user asks for help, send a message to them informing them of the operations they can perform.
            if (validCommands.includes(text)) {
                if (text === 'help') {
                    await turnContext.sendActivity(HELP_TEXT);
                }
                // Log the user out
                if (text === 'logout') {
                    let botAdapter = turnContext.adapter;
                    await botAdapter.signOutUser(turnContext, CONNECTION_NAME);
                    await turnContext.sendActivity('You have been signed out.');
                    await turnContext.sendActivity(HELP_TEXT);
                }
            } else {
                if (!turnContext.responded) {
                    await dc.beginDialog(AUTH_DIALOG);
                }
            };
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            // Send a greeting to new members that join the conversation.
            const members = turnContext.activity.membersAdded;

            for (let index = 0; index < members.length; index++) {
                const member = members[index];
                if (member.id !== turnContext.activity.recipient.id) {
                    const welcomeMessage = `Welcome to AuthenticationBot ${ member.name }. ` + HELP_TEXT;
                    await turnContext.sendActivity(welcomeMessage);
                }
            };
        } else if (turnContext.activity.type === ActivityTypes.Invoke || turnContext.activity.type === ActivityTypes.Event) {
            // This handles the MS Teams Invoke Activity sent when magic code is not used.
            // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
            // The Teams manifest schema is found here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
            // It also handles the Event Activity sent from the emulator when the magic code is not used.
            // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/
            const dc = await this.dialogs.createContext(turnContext);
            await dc.continueDialog();
            if (!turnContext.responded) {
                await dc.beginDialog(AUTH_DIALOG);
            }
        } else {
            await turnContext.sendActivity(`[${ turnContext.activity.type } event detected.]`);
        }

        // Update the conversation state before ending the turn.
        await this.conversationState.saveChanges(turnContext);
    }
}

module.exports.AuthenticationBot = AuthenticationBot;
