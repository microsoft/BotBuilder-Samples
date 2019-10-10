// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Mention,
    MessageFactory,
    TeamsActivityHandler,
} from 'botbuilder';

export class MentionsBot  extends TeamsActivityHandler {
    /*
     * You can @mention the bot from any scope and it will reply with the mention.
     */    
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            var mention = { mentioned: context.activity.from, text:`<at>${context.activity.from.name}</at>` };

            // Against Teams having a Mention in the Entities but not including that
            // mention Text in the Activity Text will result in a BadRequest.
            var replyActivity = MessageFactory.text(`Hello ${mention.text}.`);
            replyActivity.entities = [ <Mention> mention ];

            await context.sendActivity(replyActivity);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}
