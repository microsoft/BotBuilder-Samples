// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory, TurnContext } = require('botbuilder');

/**
 * A bot that responds to input from suggested actions.
 */
class SuggestedActionsBot {
    /**
     * Every conversation turn for our SuggestedActionsbot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;

            // Create an array with the valid color options.
            const validColors = ['Red', 'Blue', 'Yellow'];

            // If the `text` is in the Array, a valid color was selected and send agreement.
            if (validColors.includes(text)) {
                await turnContext.sendActivity(`I agree, ${ text } is the best color.`);
            } else {
                await turnContext.sendActivity('Please select a color.');
            }

            // After the bot has responded send the suggested actions.
            await this.sendSuggestedActions(turnContext);
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            await this.sendWelcomeMessage(turnContext);
        } else {
            await turnContext.sendActivity(`[${ turnContext.activity.type } event detected.]`);
        }
    }

    /**
     * Send a welcome message along with suggested actions for the user to click.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async sendWelcomeMessage(turnContext) {
        const that = this;

        // Define a promise that will welcome the user.
        async function welcomeUser(conversationMember) {
            // Create welcome message to send to new conversation members.
            const welcomeMessage = `Welcome to suggestedActionsBot ${ conversationMember.name }. ` +
            `This bot will introduce you to Suggested Actions. ` +
            `Please select an option:`;

            // Greet anyone that was not the target (recipient) of this message.
            // The bot is the recipient of all events from the channel, including all ConversationUpdate-type activities
            // turnContext.activity.membersAdded !== turnContext.activity.recipient.id indicates
            // a user was added to the conversation.
            if (conversationMember.id !== this.activity.recipient.id) {
                // Because the TurnContext was bound to this function, the bot can call
                // `TurnContext.sendActivity` via `this.sendActivity`;
                await this.sendActivity(welcomeMessage);
                await that.sendSuggestedActions(this);
            }
        }

        // If new members are added to the conversation, welcome them via `welcomeUser()`.
        if (turnContext.activity && turnContext.activity.membersAdded) {
            // Prepare Promises to greet the users.
            const replyPromises = turnContext.activity.membersAdded.map(welcomeUser.bind(turnContext));
            await Promise.all(replyPromises);
        }
    }

    /**
     * Send suggested actions to the user.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async sendSuggestedActions(turnContext) {
        var reply = MessageFactory.suggestedActions(['Red', 'Yellow', 'Blue'], 'What is the best color?');
        await turnContext.sendActivity(reply);
    }
}

module.exports.SuggestedActionsBot = SuggestedActionsBot;
