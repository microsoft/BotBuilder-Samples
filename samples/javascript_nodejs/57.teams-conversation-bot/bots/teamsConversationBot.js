// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    TurnContext,
    MessageFactory,
    TeamsInfo,
    TeamsActivityHandler,
    CardFactory,
    ActionTypes
} = require('botbuilder');
const TextEncoder = require('util').TextEncoder;

class TeamsConversationBot extends TeamsActivityHandler {
    constructor() {
        super();
        this.onMessage(async (context, next) => {
            TurnContext.removeRecipientMention(context.activity);
            switch (context.activity.text.trim().toLocaleLowerCase()) {
            case 'mentionme':
                await this.mentionActivityAsync(context);
                break;
            case 'updatecardaction':
                await this.cardActivityAsync(context, true);
                break;
            case 'delete':
                await this.deleteCardActivityAsync(context);
                break;
            case 'messageallmembers':
                await this.messageAllMembersAsync(context);
                break;
            case 'whoami':
                await this.getSingleMember(context);
                break;
            default:
                await this.cardActivityAsync(context, false)
                break;
            }
            await next();
        });

        this.onMembersAddedActivity(async (context, next) => {
            context.activity.membersAdded.forEach(async (teamMember) => {
                if (teamMember.id !== context.activity.recipient.id) {
                    await context.sendActivity(`Welcome to the team ${ teamMember.givenName } ${ teamMember.surname }`);
                }
            });
            await next();
        });
    }

    async cardActivityAsync(context, isUpdate) {
        const cardActions = [                       
                {
                    type: ActionTypes.MessageBack,
                    title: 'Message all members',
                    value: null,
                    text: 'MessageAllMembers'
                },
                {
                    type: ActionTypes.MessageBack,
                    title: 'Who am I?',
                    value: null,
                    text: 'whoami'
                },
                {
                    type: ActionTypes.MessageBack,
                    title: 'Delete card',
                    value: null,
                    text: 'Delete'
                }
        ];

        if(isUpdate) {
            await this.sendUpdateCard(context, cardActions);
        }
        else {
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
            text: 'UpdateCardAction'
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
            count: 0
        };
        cardActions.push({
            type: ActionTypes.MessageBack,
            title: 'Update Card',
            value: initialValue,
            text: 'UpdateCardAction'
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
        var member;
        try {
            member = await TeamsInfo.getMember(context, "banana");
        }
        catch (e) {
            if(e.code === 'MemberNotFoundInConversation') {
                context.sendActivity(MessageFactory.text('Member not found.'));
                return;
            }
            else {
                console.log(e);
                throw e;
            }
        }
        const message = MessageFactory.text(`You are: ${member.name}`);
        await context.sendActivity(message);
    }

    async mentionActivityAsync(context) {
        const mention = {
            mentioned: context.activity.from,
            text: `<at>${ new TextEncoder().encode(context.activity.from.name) }</at>`,
            type: 'mention'
        };

        const replyActivity = MessageFactory.text(`Hi ${ mention.text }`);
        replyActivity.entities = [mention];
        await context.sendActivity(replyActivity);
    }

    async deleteCardActivityAsync(context) {
        await context.deleteActivity(context.activity.replyToId);
    }

    // If you encounter permission-related errors when sending this message, see
    // https://aka.ms/BotTrustServiceUrl
    async messageAllMembersAsync(context) {
        var continuationToken;
        var members = [];

        do {
            var pagedMembers = await TeamsInfo.getPagedMembers(context, 100, continuationToken);
            continuationToken = pagedMembers.continuationToken;
            members.push(...pagedMembers.members);
        }
        while(continuationToken !== undefined)

        members.forEach(async (teamMember) => {
            const message = MessageFactory.text(`Hello ${ teamMember.givenName } ${ teamMember.surname }. I'm a Teams conversation bot.`);

            var ref = TurnContext.getConversationReference(context.activity);
            ref.user = teamMember;

            await context.adapter.createConversation(ref,
                async (t1) => {
                    const ref2 = TurnContext.getConversationReference(t1.activity);
                    await t1.adapter.continueConversation(ref2, async (t2) => {
                        await t2.sendActivity(message);
                    });
                });
        });

        await context.sendActivity(MessageFactory.text('All messages have been sent.'));
    }
}

module.exports.TeamsConversationBot = TeamsConversationBot;
