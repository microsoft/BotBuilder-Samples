const { 
    TurnContext, 
    MessageFactory, 
    Mention, 
    TeamsInfo,
    BotFrameworkAdapter,
    TeamsActivityHandler,    
    CardFactory,
    ActionTypes
} = require("botbuilder");

const { MicrosoftAppCredentials } = require("botframework-connector");
const { HeroCard } = require("botframework-connector/lib/connectorApi/models/mappers");
const { CardAction } = require("botframework-connector/lib/teams/models/mappers");
const { type } = require("os");

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class TeamsConversationBot extends TeamsActivityHandler {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     
     */
    constructor(conversationState, userState) {
        super(conversationState, userState);

        this.onMessage(async (context, next) => {
            const text = TurnContext.removeRecipientMention(context.activity).trim();

            switch(text) {
                case "MentionMe":
                    await this.MentionActivityAsync(context);
                    break;
                
                case "UpdateCardAction":
                    await this.UpdateCardActivityAsync(context);
                    break;
                
                case "Delete":
                    await this.DeleteCardActivityAsync(context);
                    break;
                
                case "MessageAllMembers":
                    await this.MessageAllMemebersAsync(context);
                    break;
                
                default:
                    const value = {"count": 0};
                    let card = CardFactory.heroCard(
                        "Welcome Card",
                        null,
                        [
                            {
                                type: ActionTypes.MessageBack,
                                title: "Update Card",
                                value: value,
                                text: "UpdateCardAction"
                            },
                            {
                                type: ActionTypes.MessageBack,
                                title: "Message all members",
                                value: null,
                                text: "MessageAllMembers",
                            }
                        ]
                        );

                    await context.sendActivity({attachments: [card] });
                    break;
            }

            await next();
        });

        this.onMembersAddedActivity(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(`Welcome to the team ${membersAdded[cnt].id}`);
                }
            }
            await next();
        });
    }

    async MentionActivityAsync(context) {
        const mention = { 
            mentioned: context.activity.from, 
            text: `<at>${context.activity.from.name}</at>`,
            type: 'mention'
        };        

        const replyActivity = MessageFactory.text(mention.text);
        replyActivity.entities = [ mention ];
        await context.sendActivity(replyActivity);
    };

    async UpdateCardActivityAsync(context) {
        const data = context.activity.value;
        data["count"] += 1;

        let card = CardFactory.heroCard(
            "Welcome Card",
            `Updated count - ${data["count"]}`,
            null,
            [
                {
                    type: ActionTypes.MessageBack,
                    title: "Update Card",
                    value: data,
                    text: "UpdateCardAction"
                },
                {
                    type: ActionTypes.MessageBack,
                    title: "Message all members",
                    value: null,
                    text: "MessageAllMembers",
                },
                {
                    type: ActionTypes.MessageBack,
                    title: "Delete card",
                    value: null,
                    text: "Delete",
                }
            ]
            );
            
            card.id = context.activity.replyToId;
            await context.updateActivity({attachments: [card], id: context.activity.replyToId, type: "message"});
    };

    async DeleteCardActivityAsync(context) {
        await context.deleteActivity(context.activity.replyToId);
    };


    async MessageAllMemebersAsync(context) {
        const context2 = JSON.parse(JSON.stringify(context));
        //console.log("Context:\n" + JSON.stringify(context));
        const members = await TeamsInfo.getMembers(context);
        const teamsChannelId = context.activity.channelId;
        //console.log("teamsChannelId:\n" + JSON.stringify(teamsChannelId));
        const serviceUrl = context.activity.serviceUrl;
        const credentials = new MicrosoftAppCredentials(process.env.MicrosoftAppId, process.env.MicrosoftAppPassword);

        members.forEach(async (member) => {
            const message = MessageFactory.text(`Hello ${member.givenName} ${member.surname}. I'm a Teams conversation bot.`);


            const conversationParameters = {
                isGroup: false,
                members: [ member ],
                tenantId: context.activity.conversation.tenantId,
                bot: context.activity.recipient
            };

            
            const adapter = context.adapter;
            const connectorClient = adapter.createConnectorClient(context.activity.serviceUrl);
            
            
            // This call does NOT send the outbound Activity is not being sent through the middleware stack.
            const conversationResourceResponse = await connectorClient.conversations.createConversation(conversationParameters);
            

            //const conversationReference = TurnContext.getConversationReference(context2.activity);

            console.log(context2._activity);
            console.log(context2._activity.id);

            const activity2 = context2._activity;
            let cr = {
                activityId: activity2.id,
                user: activity2.from,
                bot: activity2.recipient,
                conversation: activity2.conversation,
                channelId: activity2.channelId,
                serviceUrl: serviceUrl
            };


            
            cr.conversation.id = conversationResourceResponse.id;

            console.log("reference applied");
            await adapter.continueConversation(cr,
                async (t) =>
                {
                    await t.sendActivity(message);
                });

        });
        await context.sendActivity(MessageFactory.text("All messages have been sent"));
    };
}

module.exports.TeamsConversationBot = TeamsConversationBot;
