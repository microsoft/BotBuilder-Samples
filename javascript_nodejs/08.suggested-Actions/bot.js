// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory } = require('botbuilder');
/**
 * A simple bot that responds to unput from suggested actions.
 */
class SuggestedActionsBot {
    /**
     * Every conversation turn for our SuggestedActionsbot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {Object} turnContext on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;

            // Create an array with the valid color options.
            const validColors = ['Red', 'Blue', 'Yellow'];

            // If the `text` is in the Array, a valid color was selected and send agreement. 
            if (validColors.includes(text)) {
                await turnContext.sendActivity(`I agree, ${text} is the best color.`);
            } else {
                await turnContext.sendActivity('Please select a color.');
            }

            // After the bot has responded send the SuggestedActions.
            await sendSuggestedActions(turnContext);
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            let members = turnContext.activity.membersAdded;

            for (let index = 0; index < members.length; index++) {
                const member = members[index];
                if (member.id != turnContext.activity.recipient.id) {
                    const welcomeMessage = `Welcome to suggestedActionsBot ${member.name}.` +
                        ' This bot will introduce you to Suggested Actions.' +
                        ' Please select an option:';
                    await turnContext.sendActivity(welcomeMessage);
                    await sendSuggestedActions(turnContext);
                }
            };
        } else {
            await turnContext.sendActivity(`[${turnContext.activity.type} event detected.]`);
        }
    }
}

/**
 * Send the suggested actions to the user.
 * @param {Object} turnContext on turn context object.
 */
async function sendSuggestedActions(turnContext) {
    var reply = MessageFactory.suggestedActions(['Red', 'Yellow', 'Blue'], `What is the best color?`);
    await turnContext.sendActivity(reply);
}


module.exports = SuggestedActionsBot;