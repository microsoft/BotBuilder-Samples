// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Activity,
    MessageFactory,
    TeamsActivityHandler,
    TeamsInfo,
    TurnContext
} from 'botbuilder';

//
// You need to install this bot in a team. You can @mention the bot "show members", "show channels", or "show details" to get the
// members of the team, the channels of the team, or metadata about the team respectively.
//
export class RosterBot extends TeamsActivityHandler {
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(MessageFactory.text(`Echo: ${context.activity.text}`));
            TurnContext.removeRecipientMention(context.activity);
            const text = context.activity.text ? context.activity.text.trim() : '';
            switch (text) {
                case "show members":
                    await this.showMembers(context);
                    break;

                case "show channels":
                    await this.showChannels(context);
                    break;

                case "show details":
                    await this.showDetails(context);
                    break;

                default:
                    await context.sendActivity(
                        'Invalid command. Type "Show channels" to see a channel list. Type "Show members" to see a list of members in a team. ' +
                        'Type "show group chat members" to see members in a group chat.');
                    break;
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello and welcome!');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    private async showMembers(context: TurnContext): Promise<void> {
        let teamsChannelAccounts = await TeamsInfo.getMembers(context);
        await context.sendActivity(MessageFactory.text(`Total of ${teamsChannelAccounts.length} members are currently in team`));
        let messages = teamsChannelAccounts.map(function(teamsChannelAccount) {
            return `${teamsChannelAccount.aadObjectId} --> ${teamsChannelAccount.name} --> ${teamsChannelAccount.userPrincipalName}`;
        });
        await this.sendInBatches(context, messages);
    }
    
    private async showChannels(context: TurnContext): Promise<void> { 
        let channels = await TeamsInfo.getChannels(context);
        await context.sendActivity(MessageFactory.text(`Total of ${channels.length} channels are currently in team`));
        let messages = channels.map(function(channel) {
            return `${channel.id} --> ${channel.name ? channel.name : 'General'}`;
        });
        await this.sendInBatches(context, messages);
    }
   
    private async showDetails(context: TurnContext): Promise<void> {
        let teamDetails = await TeamsInfo.getTeamDetails(context);
        await context.sendActivity(MessageFactory.text(`The team name is ${teamDetails.name}. The team ID is ${teamDetails.id}. The AAD GroupID is ${teamDetails.aadGroupId}.`));
    }

    private async sendInBatches(context: TurnContext, messages: string[]): Promise<void> {
        let batch: string[] = [];
        messages.forEach(async (msg: string) => {
            batch.push(msg);
            if (batch.length == 10) {
                await context.sendActivity(MessageFactory.text(batch.join('<br>')));
                batch = [];
            }
        });

        if (batch.length > 0) {
            await context.sendActivity(MessageFactory.text(batch.join('<br>')));
        }
    }
}
