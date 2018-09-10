// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory } = require('botbuilder');
const { ChoicePrompt, DialogSet, WaterfallDialog, OAuthPrompt } = require('botbuilder-dialogs');
const DIALOG_STATE_PROPERTY = 'dialogState';
const OAUTH_PROMPT = 'oAuth_prompt';
const CONFIRM_PROMPT = 'confirm_prompt';
const AUTH_DIALOG = 'auth_dialog';
const HELP_TEXT = ' Type anything to get logged in. Type \'logout\' to signout.' +
    ' Type \'help\' to view this message again';

// The connection name here must match the the one from
// your Bot Channels Registration on the settings blade in Azure.
const CONNECTION_NAME = "";

// Create the settings for the OAuthPrompt.
const OAUTH_SETTINGS = {
    connectionName: CONNECTION_NAME,
    title: "Sign In",
    text: "Please Sign In",
    timeout: 300000
};


/**
 * A simple bot that authenticates users using OAuth prompts.
 */
class AuthenticationBot {

    /**
     * The constructor for the bot. 
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     */
    constructor(conversationState) {

        // Create a new state accessor property. See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.conversationState = conversationState;
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.dialogs = new DialogSet(this.dialogState);

        // Add prompts that will be used by the bot.

        this.dialogs.add(new ChoicePrompt(CONFIRM_PROMPT));
        this.dialogs.add(new OAuthPrompt(OAUTH_PROMPT, OAUTH_SETTINGS))

        // The waterfall dialog that controls the flow of the conversation.
        this.dialogs.add(new WaterfallDialog(AUTH_DIALOG, [
            async(dc) => {
                return await dc.prompt(OAUTH_PROMPT);
            },
            async(dc, step) => {
                let tokenResponse = step.result;
                if (tokenResponse != null) {
                    await dc.context.sendActivity("You are now logged in.");
                    return await dc.prompt(CONFIRM_PROMPT, 'Do you want to view your token?', ['yes', 'no']);
                }

                await dc.Context.sendActivity("Login was not sucessful please try again");
                return await dc.end();
            },
            async(dc, step) => {
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
                    let prompt = await dc.prompt(OAUTH_PROMPT);
                    var tokenResponse = prompt.result;
                    if (tokenResponse != null) {
                        await dc.context.sendActivity(`Here is your token: ${tokenResponse.token}`);
                        await dc.context.sendActivity(HELP_TEXT);
                        return await dc.end();
                    }
                }

                await dc.context.sendActivity(HELP_TEXT);
                return await dc.end();
            },
        ]));
    }

    /**
     * Every conversation turn for our AuthenticationBot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {Object} turnContext on turn context object.
     */
    async onTurn(turnContext) {

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Create a dialog context object.
            const dc = await this.dialogs.createContext(turnContext);
            const text = turnContext.activity.text;

            // Create an array with the valid color options.
            const validCommands = ['logout', 'help'];
            await dc.continue();

            // If the `text` is in the Array, a valid color was selected and send agreement. 
            if (validCommands.includes(text)) {
                if (text === 'help') {
                    await turnContext.sendActivity(HELP_TEXT);
                }

                if (text === 'logout') {
                    let botAdapter = turnContext.adapter;
                    await botAdapter.signOutUser(turnContext, CONNECTION_NAME);
                    await turnContext.sendActivity("You have been signed out.");
                    await turnContext.sendActivity(HELP_TEXT)
                }
            } else {
                if (!turnContext.responded) {
                    await dc.begin(AUTH_DIALOG);
                }
            };
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            // Send a greeting to new members that join the conersation.
            let members = turnContext.activity.membersAdded;

            for (let index = 0; index < members.length; index++) {
                const member = members[index];
                if (member.id != turnContext.activity.recipient.id) {
                    const welcomeMessage = `Welcome to AuthenticationBot ${member.name}. ` + HELP_TEXT;
                    await turnContext.sendActivity(welcomeMessage);
                }
            };
        } else if (turnContext.activity.type === ActivityTypes.Invoke || turnContext.activity.type === ActivityTypes.Event) {
            // This handles the MS Teams Invoke Activity sent when magic code is not used.
            // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
            // The Teams manifest schema is found : https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
            // It also handles the Event Activity sent from the emulator when the magic code is not used.
            // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/
            const dc = await this.dialogs.createContext(turnContext);
            await dc.continue;
            if (!turnContext.responded) {
                await dc.begin(AUTH_DIALOG);
            }
        } else {
            await turnContext.sendActivity(`[${turnContext.activity.type} event detected.]`);
        }
    }
}

module.exports = AuthenticationBot;