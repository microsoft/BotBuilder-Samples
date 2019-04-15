// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityHandler } from 'botbuilder';

export class MyBot extends ActivityHandler {
    constructor() {
        super();
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`You said '${ context.activity.text }'`);
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
        this.onConversationUpdate(async (context, next) => {
            await context.sendActivity('[conversationUpdate event detected]');
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}
