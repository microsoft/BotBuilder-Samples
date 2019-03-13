// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder-core');
const WelcomeCard = require('./resources/welcomeCard.json');

module.exports = function(bot) {
    // Bind a handler function that will fire whenever 1 or more users joins the conversation.
    // This function will send a welcome card to each member, excluding the bot itself.
    bot.onMembersAdded(async context => {
        // Iterate over each member being added...
        for (let m = 0; m < context.activity.membersAdded.length; m++) {
            const member = context.activity.membersAdded[m];

            // Make sure this isn't the bot itself...
            if (member.id !== context.activity.recipient.id) {
                // Create and send a welcome card...
                const welcomeCard = CardFactory.adaptiveCard(WelcomeCard)
                await context.sendActivity({ attachments: [welcomeCard] });
            }
        }
    });
}

