// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, TurnContext } = require('botbuilder');


class ProactiveBot extends ActivityHandler {
    constructor(conversationReferences) {
        super();

        this.conversationReferences = conversationReferences;

        this.onConversationUpdate(async (context, next) => {

            const conversationReference = TurnContext.getConversationReference(context.activity);
            this.conversationReferences[conversationReference.conversation.id] = conversationReference;
            
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    const welcomeMessage = "Welcome to the Proactive Bot sample.  Navigate to http://localhost:3978/api/notify to proactively message everyone who has previously messaged this bot.";
                    await context.sendActivity(welcomeMessage);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.ProactiveBot = ProactiveBot;
