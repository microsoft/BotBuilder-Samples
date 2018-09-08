// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory } = require('botbuilder');

class SuggestedActionsBot {
    /**
     * 
     * @param {Object} turnContext on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;
            switch (text) {
                case 'Red':
                    await sendResponse(turnContext);
                    break;
                case 'Yellow':
                    await sendResponse(turnContext);
                    break;
                case 'Blue':
                    await sendResponse(turnContext);
                    break;
                default:
                    await turnContext.sendActivity('Please select a color.');
                    break;
            }
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
            await turnContext.sendActivity(`[${turnContext.activity.type} event detected]`);
        }
    }
}

async function sendResponse(turnContext) {
    await turnContext.sendActivity(`I agree, ${turnContext.activity.text} is the best color`);
}
/**
 * 
 * @param {Object} turnContext on turn context object.
 */
async function sendSuggestedActions(turnContext) {
    var reply = MessageFactory.suggestedActions(['Red', 'Yellow', 'Blue'], `What is the best color`);
    await turnContext.sendActivity(reply);
}


module.exports = SuggestedActionsBot;