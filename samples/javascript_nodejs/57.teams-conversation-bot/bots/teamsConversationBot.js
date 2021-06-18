// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    TurnContext,
    MessageFactory,
    TeamsInfo,
    TeamsActivityHandler,
    CardFactory,
    ActionTypes,
} = require('botbuilder');
const TextEncoder = require('util').TextEncoder;

class TeamsConversationBot extends TeamsActivityHandler {
    constructor() {
        super();

        this.onMessage(async (context, next) => {
            TurnContext.removeRecipientMention(context.activity);

            const text = context.activity.text.trim().toLocaleLowerCase();
            if (text.includes('mention')) {
                await this.mentionActivityAsync(context);
            } else if (text.includes('update')) {
                await this.cardActivityAsync(context, true);
            } else if (text.includes('delete')) {
                await this.deleteCardActivityAsync(context);
            } else if (text.includes('message')) {
                await this.messageAllMembersAsync(context);
            } else if (text.includes('who')) {
                await this.getSingleMember(context);
            } else {
                await this.cardActivityAsync(context, false);
            }

            await next();
        });

        this.onMembersAddedActivity(async (context, next) => {
            await Promise.all((context.activity.membersAdded || []).map(async (member) => {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity(
                        `Welcome to the team ${member.givenName} ${member.surname}`
                    );
                }
            }));

            await next();
        });

        this.onReactionsAdded(async (context) => {
            await Promise.all((context.activity.reactionsAdded || []).map(async (reaction) => {
                const newReaction = `You reacted with '${reaction.type}' to the following message: '${context.activity.replyToId}'`;
                const resourceResponse = await context.sendActivity(newReaction);
                // Save information about the sent message and its ID (resourceResponse.id).
            }));
        });

        this.onReactionsRemoved(async (context) => {
            await Promise.all((context.activity.reactionsRemoved || []).map(async (reaction) => {
                const newReaction = `You removed the reaction '${reaction.type}' from the message: '${context.activity.replyToId}'`;
                const resourceResponse = await context.sendActivity(newReaction);
                // Save information about the sent message and its ID (resourceResponse.id).
            }));
        });
    }

    async cardActivityAsync(context, isUpdate) {
        const cardActions = [
            {
                type: ActionTypes.MessageBack,
                title: 'Message all members',
                value: null,
                text: 'MessageAllMembers',
            },
            {
                type: ActionTypes.MessageBack,
                title: 'Who am I?',
                value: null,
                text: 'whoami',
            },
            {
                type: ActionTypes.MessageBack,
                title: 'Delete card',
                value: null,
                text: 'Delete',
            },
        ];

        if (isUpdate) {
            await this.sendUpdateCard(context, cardActions);
        } else {
            await this.sendWelcomeCard(context, cardActions);
        }
    }

    async sendUpdateCard(context, cardActions) {
        const data = context.activity.value;
        data.count += 1;
        cardActions.push({
            type: ActionTypes.MessageBack,
            title: 'Update Card',
            value: data,
            text: 'UpdateCardAction',
        });
        const card = CardFactory.heroCard(
            'Updated card',
            `Update count: ${data.count}`,
            null,
            cardActions
        );
        card.id = context.activity.replyToId;
        const message = MessageFactory.attachment(card);
        message.id = context.activity.replyToId;
        await context.updateActivity(message);
    }

    async sendWelcomeCard(context, cardActions) {
        const initialValue = {
            count: 0,
        };
        cardActions.push({
            type: ActionTypes.MessageBack,
            title: 'Update Card',
            value: initialValue,
            text: 'UpdateCardAction',
        });
        const card = CardFactory.heroCard(
            'Welcome card',
            '',
            null,
            cardActions
        );
        await context.sendActivity(MessageFactory.attachment(card));
    }

    async getSingleMember(context) {
        try {
            const member = await TeamsInfo.getMember(
                context,
                context.activity.from.id
            );
            const message = MessageFactory.text(`You are: ${member.name}`);
            await context.sendActivity(message);
        } catch (e) {
            if (e.code === 'MemberNotFoundInConversation') {
                return context.sendActivity(MessageFactory.text('Member not found.'));
            } else {
                throw e;
            }
        }
    }

    async mentionActivityAsync(context) {
        const mention = {
            mentioned: context.activity.from,
            text: `<at>${new TextEncoder().encode(
                context.activity.from.name
            )}</at>`,
            type: 'mention',
        };

        const replyActivity = MessageFactory.text(`Hi ${mention.text}`);
        replyActivity.entities = [mention];
        await context.sendActivity(replyActivity);
    }

    async deleteCardActivityAsync(context) {
        await context.deleteActivity(context.activity.replyToId);
    }

    async messageAllMembersAsync(context) {
        const members = await this.getPagedMembers(context);

        await Promise.all(members.map(async (member) => {
            const message = MessageFactory.text(
                `Hello ${member.givenName} ${member.surname}. I'm a Teams conversation bot.`
            );

            const ref = TurnContext.getConversationReference(context.activity);
            ref.user = member;

            await context.adapter.createConversation(ref, async (context) => {
                const ref = TurnContext.getConversationReference(context.activity);

                await context.adapter.continueConversation(ref, async (context) => {
                    await context.sendActivity(message);
                });
            });
        }));

        await context.sendActivity(MessageFactory.text('All messages have been sent.'));
    }

    async getPagedMembers(context) {
        let continuationToken;
        const members = [];

        do {
            const page = await TeamsInfo.getPagedMembers(
                context,
                100,
                continuationToken
            );

            continuationToken = page.continuationToken;

            members.push(...page.members);
        } while (continuationToken !== undefined);

        return members;
    }

    async onTeamsChannelCreated(context) {
        const card = CardFactory.heroCard(
            'Channel Created',
            `${context.activity.channelData.channel.name} is new the Channel created`
        );
        const message = MessageFactory.attachment(card);
        await context.sendActivity(message);
    }

    async onTeamsChannelRenamed(context) {
        const card = CardFactory.heroCard(
            'Channel Renamed',
            `${context.activity.channelData.channel.name} is the new Channel name`
        );
        const message = MessageFactory.attachment(card);
        await context.sendActivity(message);
    }

    async onTeamsChannelDeleted(context) {
        const card = CardFactory.heroCard(
            'Channel Deleted',
            `${context.activity.channelData.channel.name} is deleted`
        );
        const message = MessageFactory.attachment(card);
        await context.sendActivity(message);
    }

    async onTeamsChannelRestored(context) {
        const card = CardFactory.heroCard(
            'Channel Restored',
            `${context.activity.channelData.channel.name} is the Channel restored`
        );
        const message = MessageFactory.attachment(card);
        await context.sendActivity(message);
    }

    async onTeamsTeamRenamed(context) {
        const card = CardFactory.heroCard(
            'Team Renamed',
            `${context.activity.channelData.team.name} is the new Team name`
        );
        const message = MessageFactory.attachment(card);
        await context.sendActivity(message);
    }
}

module.exports.TeamsConversationBot = TeamsConversationBot;
