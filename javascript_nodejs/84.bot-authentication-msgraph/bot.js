// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, BotFrameworkAdapter, CardFactory } = require('botbuilder');
const { DialogSet, OAuthPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { OAuthHelpers } = require('./oauth-helpers');

/**
 * This bot uses OAuth to log the user in. The OAuth provider being demonstrated
 * here is Azure Active Directory v2.0 (AADv2). Once logged in, the bot uses the
 * Microsoft Graph API to demonstrate making calls to authenticated services.
 */
class GraphAuthenticationBot {
    /**
     * Constructs the three pieces necessary by this bot to operate:
     * 1. StatePropertyAccessor
     * 2. DialogSet
     * 3. OAuthPrompt
     * 
     * The arguments taken by this constructor are ConversationState and UserState, although any BotState
     * instance would suffice for this bot. `conversationState` is used to create a StatePropertyAccessor,
     *  which is needed to create a DialogSet. All botbuilder-dialogs `Prompts` need a DialogSet to operate.
     * @param {ConversationState} conversationState 
     * @param {UserState} userState
     */
    constructor(conversationState, userState) {
        this.data = conversationState;
        this.user = userState;
        const that = this;

        // DialogState property accessor. Used to keep persist DialogState when using DialogSet.
        this.dialogState = conversationState.createProperty("dialogState");
        this.commandState = conversationState.createProperty('commandState');

        // Create a DialogSet that contains the OAuthPrompt.
        this.dialogs = new DialogSet(this.dialogState);

        // Instructions for the user with information about commands that this bot may handle.
        this.helpMessage = `You can type "send <recipient_email>" to send an email, "recent" to view recent unread mail,` +
            ` "me" to see information about your, or "help" to view the commands` +
            ` again. Any other text will display your token.`;

        this.connectionSettingName = 'AADv4Node';
        this.dialogs.add(OAuthHelpers.prototype.prompt(this.connectionSettingName));

        // Logs in the user and calls proceeding dialogs, if login is successful.
        this.dialogs.add(new WaterfallDialog('graphDialog', [
            this.promptStep.bind(this),
            this.processStep.bind(this)
        ]));
    };

    // Move this after onTurn

    /**
     * Sends a welcome hero card.
     * @param {Object} turnContext 
     */
    async sendWelcomeMessage(turnContext) {
        // Creates a Hero Card that is sent as a welcome message to the user.
        // this.heroCard = function () {
        const heroCard = CardFactory.heroCard(
            "Welcome",
            CardFactory.images(["https://botframeworksamples.blob.core.windows.net/samples/aadlogo.png"]),
            CardFactory.actions([
                {
                    "type": "imBack",
                    "title": "Me",
                    "value": "Me"
                },
                {
                    "type": "imBack",
                    "title": "Recent",
                    "value": "Recent"
                },
                {
                    "type": "imBack",
                    "title": "View Token",
                    "value": "View Token"
                },
                {
                    "type": "imBack",
                    "title": "Help",
                    "value": "Help"
                },
                {
                    "type": "imBack",
                    "title": "Signout",
                    "value": "Signout"
                }
            ])
        );
        await turnContext.sendActivity({ attachments: [heroCard] });
    }


    /**
     * This controls what happens when an activity get sent to the bot.
     * @param {String} turnContext Provides the turnContext for the turn of the bot.
     */
    async onTurn(turnContext) {
        const dc = await this.dialogs.createContext(turnContext);

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        switch (turnContext.activity.type) {
            case ActivityTypes.Message:
                await this.processInput(dc);
            case ActivityTypes.Event:
            case ActivityTypes.Invoke:
                // This handles the Microsoft Teams Invoke Activity sent when magic code is not used.
                // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
                // Manifest Schema Here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
                // It also handles the Event Activity sent from The Emulator when the magic code is not used.
                // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/

                // Sanity check the Activity type and channel Id.
                if (turnContext.activity.type === ActivityTypes.Invoke && turnContext.activity.channelId !== "msteams") {
                    throw new Error("The Invoke type is only valid on the MS Teams channel.");
                };
                await dc.continue();
                if (!turnContext.responded) {
                    await dc.begin("graphDialog");
                };
            case ActivityTypes.ConversationUpdate:
                const members = turnContext.activity.membersAdded;
                for (const member in members) {
                    if (members[0].name !== "Bot") {
                        // Send a HeroCard as a welcome message when a new user joins the conversation.
                        await this.sendWelcomeMessage(turnContext);
                        // await dc.begin("graphDialog");
                    }
                };
        }
    };

    // Processes input and route to the appropriate step.
    async processInput(dc) {
        switch (dc.context.activity.text.toLowerCase()) {
            case 'signout':
            case 'logout':
            case 'signoff':
            case 'logoff':
                // The bot adapter encapsulates the authentication processes and sends
                // activities to from the Bot Connector Service.
                const botAdapter = dc.context.adapter;
                await botAdapter.signOutUser(dc.context, this.connectionSettingName);
                // Let the user know they are signed out.
                await dc.context.sendActivity('You are now signed out.');
                break;
            case 'help':
                await dc.context.sendActivity(this.helpMessage);
                break;
            default:
                // The user has input a command that has not been handled yet,
                // begin the waterfall dialog to handle the input.
                await dc.continue();
                if (!dc.context.responded) {
                    await dc.begin('graphDialog', dc);
                }
        }
        return dc;
    };

    /**
     * WaterfallDialogStep for storing commands and beginning the OAuthPrompt.
     * Saves the user's message as the command to execute if the message is not
     * a magic code.
     * @param {Object} step WaterfallStepContext
     */
    async promptStep(step) {
        const activity = step.context.activity;

        if (activity.type === ActivityTypes.Message && !(/\d{6}/).test(activity.text)) {
            await this.commandState.set(step.context, activity.text);
        }
        return await step.begin('loginPrompt');
    }

    /**
     * WaterfallDialogStep to process the command sent by the user.
     * @param {Object} step WaterfallStepContext
     */
    async processStep(step) {
        console.log(step);
        // We do not need to store the token in the bot. When we need the token we can
        // send another prompt. If the token is valid the user will not need to log back in.
        // The token will be available in the Result property of the task.
        const tokenResponse = step.result;

        // If we have the token use the user is authenticated so we may use it to make API calls.
        if (tokenResponse !== undefined) {
            const parts = (step.context.activity.text).split(" ");
            const command = parts[0].toLowerCase();
            if (command == "me") {
                await OAuthHelpers.prototype.listMe(step, tokenResponse);
                return await step.end();
            } else if (command == "send") {
                await OAuthHelpers.prototype.sendMail(step, tokenResponse, parts[1].toLowerCase())
                return await step.end();
            } else if (command == "recent") {
                await OAuthHelpers.prototype.listRecentMail(step, tokenResponse);
                return await step.end();
            } else {
                await step.context.sendActivity(`Your token is: ${tokenResponse.token}`);
                return await step.end();
            }
        } else {
            step.context.sendActivity(`We couldn't log you in. Please try again later.`);
        }
    };
};

exports.GraphAuthenticationBot = GraphAuthenticationBot;
