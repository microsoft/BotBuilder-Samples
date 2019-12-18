// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActivityTypes, EndOfConversationCodes } = require('botbuilder');

class EchoBot extends ActivityHandler {
    constructor() {
        super();
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            if (context.activity.text.toLowerCase().includes('end') || context.activity.text.toLowerCase().includes('stop')) {
                const endOfConversation = {
                    type: ActivityTypes.EndOfConversation,
                    code: EndOfConversationCodes.CompletedSuccessfully
                };
                await context.sendActivity(endOfConversation);
            } else {
                await context.sendActivity(`Echo (JS) : '${ context.activity.text }'`);
                await context.sendActivity('Say "end" or "stop" and I\'ll end the conversation and back to the parent.');
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.EchoBot = EchoBot;
