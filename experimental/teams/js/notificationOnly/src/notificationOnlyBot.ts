// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ChannelAccount,
    MessageFactory,
    TeamsActivityHandler,
    TeamInfo,
    TurnContext,
} from 'botbuilder';

export class NotificationOnlyBot extends TeamsActivityHandler {
    /*
     * This bot needs to be installed in a team or group chat that you are an admin of. You can add/remove someone from that team and
     * the bot will send that person a 1:1 message saying what happened. Also, yes, this scenario isn't the most up to date with the updated
     * APIs for membersAdded/removed. Also you should NOT be able to @mention this bot.
     */    
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`You said '${context.activity.text}'`);
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onTeamsMembersAddedEvent(async (membersAdded: ChannelAccount[], teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            for (const member of membersAdded) {
                var replyActivity = MessageFactory.text(`${member.id} was added to the team.`);
                replyActivity = TurnContext.applyConversationReference(replyActivity, TurnContext.getConversationReference(context.activity));
                await context.sendActivity(replyActivity);
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
        this.onTeamsMembersRemovedEvent(async (membersRemoved: ChannelAccount[], teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            for (const member of membersRemoved) {
                var replyActivity = MessageFactory.text(`${member.id} was removed from the team.`);
                replyActivity = TurnContext.applyConversationReference(replyActivity, TurnContext.getConversationReference(context.activity));
                await context.sendActivity(replyActivity);
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
        this.onMembersAdded(async (context: TurnContext, next: () => Promise<void>): Promise<void> => {
            for (const member of context.activity.membersAdded) {
                var replyActivity = MessageFactory.text(`${member.id} was added to the team.`);
                
                replyActivity = TurnContext.applyConversationReference(replyActivity, TurnContext.getConversationReference(context.activity));
                const channelId = context.activity.conversation.id.split(';')[0];
                replyActivity.conversation.id = channelId;
                await context.sendActivity(replyActivity);
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
        this.onMembersRemoved(async (context: TurnContext, next: () => Promise<void>): Promise<void> => {
            for (const member of context.activity.membersRemoved) {
                var replyActivity = MessageFactory.text(`${member.id} was removed from the team.`);
                
                replyActivity = TurnContext.applyConversationReference(replyActivity, TurnContext.getConversationReference(context.activity));
                const channelId = context.activity.conversation.id.split(';')[0];
                replyActivity.conversation.id = channelId;
                await context.sendActivity(replyActivity);
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

}