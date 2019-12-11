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
            switch (context.activity.text.trim()) {
            case 'MentionMe':
                await this.mentionActivityAsync(context);
                break;
            case 'UpdateCardAction':
                await this.updateCardActivityAsync(context);
                break;
            case 'Delete':
                await this.deleteCardActivityAsync(context);
                break;
            case 'MessageAllMembers':
                await this.messageAllMembersAsync(context);
                break;
            default:
                const value = { count: 0 };
                const card = CardFactory.heroCard(
                    'Welcome Card',
                    null,
                    [
                        {
                            type: ActionTypes.MessageBack,
                            title: 'Update Card',
                            value: value,
                            text: 'UpdateCardAction'
                        },
                        {
                            type: ActionTypes.MessageBack,
                            title: 'Message all members',
                            value: null,
                            text: 'MessageAllMembers'
                        }]);
                await context.sendActivity({ attachments: [card] });
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

    async updateCardActivityAsync(context) {
        const data = context.activity.value;
        data.count += 1;

        const card = CardFactory.heroCard(
            'Welcome Card',
            `Updated count - ${ data.count }`,
            null,
            [
                {
                    type: ActionTypes.MessageBack,
                    title: 'Update Card',
                    value: data,
                    text: 'UpdateCardAction'
                },
                {
                    type: ActionTypes.MessageBack,
                    title: 'Message all members',
                    value: null,
                    text: 'MessageAllMembers'
                },
                {
                    type: ActionTypes.MessageBack,
                    title: 'Delete card',
                    value: null,
                    text: 'Delete'
                }
            ]);

        card.id = context.activity.replyToId;
        await context.updateActivity({ attachments: [card], id: context.activity.replyToId, type: 'message' });
    }

    async deleteCardActivityAsync(context) {
        await context.deleteActivity(context.activity.replyToId);
    }

    async messageAllMembersAsync(context) {
        const members = await TeamsInfo.getMembers(context);

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
