// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    MessageFactory,
    TeamsActivityHandler,
    teamsCreateConversation,
    teamsGetChannelId,
    BotFrameworkAdapter,
} from 'botbuilder';
import { basename } from 'path';

export class ReplyToChannelBot extends TeamsActivityHandler {
    botId: string;
    
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {

            const teamChannelId = teamsGetChannelId(context.activity);
            const message = MessageFactory.text("good morning");
            const newConversation = await teamsCreateConversation(context, teamChannelId, message);

            const adapter = context.adapter as BotFrameworkAdapter;

            await adapter.continueConversation(newConversation[0],
                async (t) =>
                {
                    await t.sendActivity(MessageFactory.text("good afternoon"));
                    await t.sendActivity(MessageFactory.text("good night"));
                });

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}
