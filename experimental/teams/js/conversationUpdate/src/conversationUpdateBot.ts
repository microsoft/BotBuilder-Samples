// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    CardFactory,
    ChannelAccount,
    ChannelInfo,
    MessageFactory,
    TeamInfo,
    TeamsActivityHandler,
    TurnContext,
} from 'botbuilder';


export class ConversationUpdateBot  extends TeamsActivityHandler {
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity('You said ' + context.activity.text);
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
        this.onTeamsChannelRenamedEvent(async (channelInfo: ChannelInfo, teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            const card = CardFactory.heroCard('Channel Renamed', `${channelInfo.name} is the new Channel name`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onTeamsChannelCreatedEvent(async (channelInfo: ChannelInfo, teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            const card = CardFactory.heroCard('Channel Created', `${channelInfo.name} is the Channel created`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onTeamsChannelDeletedEvent(async (channelInfo: ChannelInfo, teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            const card = CardFactory.heroCard('Channel Deleted', `${channelInfo.name} is the Channel deleted`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onTeamsTeamRenamedEvent(async (teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            const card = CardFactory.heroCard('Team Renamed', `${teamInfo.name} is the new Team name`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onTeamsMembersAddedEvent(async (membersAdded: ChannelAccount[], teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            var newMembers: string = '';
            console.log(JSON.stringify(membersAdded));
            membersAdded.forEach((account) => {
                newMembers.concat(account.id,' ');
            });
            const card = CardFactory.heroCard('Account Added', `${newMembers} joined ${teamInfo.name}.`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onTeamsMembersRemovedEvent(async (membersRemoved: ChannelAccount[], teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void>): Promise<void> => {
            var removedMembers: string = '';
            console.log(JSON.stringify(membersRemoved));
            membersRemoved.forEach((account) => {
                removedMembers += account.id + ' ';
            });
            const card = CardFactory.heroCard('Account Removed', `${removedMembers} removed from ${teamInfo.name}.`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onMembersAdded(async (context: TurnContext, next: () => Promise<void>): Promise<void> => {
            var newMembers: string = '';
            context.activity.membersAdded.forEach((account) => {
                newMembers += account.id + ' ';
            });
            const card = CardFactory.heroCard('Member Added', `${newMembers} joined ${context.activity.conversation.conversationType}.`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
        this.onMembersRemoved(async (context: TurnContext, next: () => Promise<void>): Promise<void> => {
            var removedMembers: string = '';
            context.activity.membersRemoved.forEach((account) => {
                removedMembers += account.id + ' ';
            });
            const card = CardFactory.heroCard('Member Removed', `${removedMembers} removed from ${context.activity.conversation.conversationType}.`);
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);
            await next();
        });
    }
}
