// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');
const { DialogBot } = require('./dialogBot');
const WelcomeCard = require('./resources/welcomeCard.json');

class DialogAndWelcomeBot extends DialogBot {
    /**
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     * @param {Dialog} dialog
     */
    constructor(conversationState, userState, dialog) {
        super(conversationState, userState, dialog);

        this.onMembersAdded(async (turnContext, next) => {
            const membersAdded = turnContext.activity.membersAdded;

            for (let pos = 0; pos < membersAdded.length; pos++) {
                if (membersAdded[pos].id !== turnContext.activity.recipient.id) {
                    const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);

                    await turnContext.sendActivity({ attachments: [welcomeCard] });
                    await dialog.run(turnContext, conversationState.createProperty('DialogState'));
                }
            }
            // Ensure next BotHandler is executed.
            await next();
        });
    }
}

module.exports.DialogAndWelcomeBot = DialogAndWelcomeBot;
