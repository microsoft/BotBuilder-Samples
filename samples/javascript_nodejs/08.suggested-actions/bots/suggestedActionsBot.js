// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, MessageFactory } = require('botbuilder');

class SuggestedActionsBot extends ActivityHandler {
    constructor() {
        super();

        this.onMembersAdded(async (context, next) => {
            await this.sendWelcomeMessage(context);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMessage(async (context, next) => {
            const text = context.activity.text;

            // Create an array with the valid color options.
            const validColors = ['Red', 'Blue', 'Yellow'];

            // If the `text` is in the Array, a valid color was selected and send agreement.
            if (validColors.includes(text)) {
                await context.sendActivity(`I agree, ${ text } is the best color.`);
            } else {
                await context.sendActivity('Please select a color.');
            }

            // After the bot has responded send the suggested actions.
            await this.sendSuggestedActions(context);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    /**
     * Send a welcome message along with suggested actions for the user to click.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async sendWelcomeMessage(turnContext) {
        const { activity } = turnContext;

        // Iterate over all new members added to the conversation.
        for (const idx in activity.membersAdded) {
            if (activity.membersAdded[idx].id !== activity.recipient.id) {
                const welcomeMessage = `Welcome to suggestedActionsBot ${ activity.membersAdded[idx].name }. ` +
                    `This bot will introduce you to Suggested Actions. ` +
                    `Please select an option:`;
                await turnContext.sendActivity(welcomeMessage);
                await this.sendSuggestedActions(turnContext);
            }
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
