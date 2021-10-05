// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    TurnContext,
    MessageFactory,
    TeamsActivityHandler,
    teamsGetChannelId
} = require('botbuilder');

class TeamsStartNewThreadInChannel extends TeamsActivityHandler {
    constructor() {
        super();

        this.onMessage(async (context, next) => {
            const teamsChannelId = teamsGetChannelId(context.activity);
            const message = MessageFactory.text('This will be the first message in a new thread');
            const newConversation = await this.teamsCreateConversation(context, teamsChannelId, message);

            await context.adapter.continueConversationAsync(process.env.MicrosoftAppId, newConversation[0], async turnContext => {
                await turnContext.sendActivity(MessageFactory.text('This will be the first response to the new thread'));
            });

            await next();
        });
    }

    async teamsCreateConversation(context, teamsChannelId, message) {
        const conversationParameters = {
            isGroup: true,
            channelData: {
                channel: {
                    id: teamsChannelId
                }
            },

            activity: message
        };

        const connectorFactory = context.turnState.get(context.adapter.ConnectorFactoryKey);
        const connectorClient = await connectorFactory.create(context.activity.serviceUrl);

        const conversationResourceResponse = await connectorClient.conversations.createConversation(conversationParameters);
        const conversationReference = TurnContext.getConversationReference(context.activity);
        conversationReference.conversation.id = conversationResourceResponse.id;
        return [conversationReference, conversationResourceResponse.activityId];
    }
}

module.exports.TeamsStartNewThreadInChannel = TeamsStartNewThreadInChannel;
