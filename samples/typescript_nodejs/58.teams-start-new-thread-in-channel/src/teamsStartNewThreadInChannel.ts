// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    Activity,
    BotFrameworkAdapter,
    ConversationParameters,
    MessageFactory,
    TeamsActivityHandler,
    teamsGetChannelId,
    TurnContext
} from 'botbuilder';
import { ChannelAccount, ConversationReference } from 'botframework-schema';

export class TeamsStartNewThreadInChannel extends TeamsActivityHandler {
    constructor() {
        super();
        this.onMessage( async ( context: TurnContext, next ): Promise<void> => {
            const teamsChannelId = teamsGetChannelId( context.activity );
            const channelAccount = context.activity.from as ChannelAccount;
            const message = MessageFactory.text( 'This will be the first message in a new thread' );
            const newConversation = await this.teamsCreateConversation( context, channelAccount, teamsChannelId, message );

            await context.adapter.continueConversation( newConversation[ 0 ],
                async ( t ) => {
                    await t.sendActivity( MessageFactory.text( 'This will be the first response to the new thread' ) );
                } );

            await next();
        } );
    }

    public async teamsCreateConversation( context: TurnContext, channelAccount: ChannelAccount, teamsChannelId: string, message: Partial<Activity> ): Promise<any> {
        const conversationParameters = {
            bot: channelAccount,
            channelData: {
                channel: {
                    id: teamsChannelId
                }
            },
            isGroup: true,

            activity: message
        } as ConversationParameters;

        const botAdapter = context.adapter as BotFrameworkAdapter;
        const connectorClient = botAdapter.createConnectorClient( context.activity.serviceUrl );
        const conversationResourceResponse = await connectorClient.conversations.createConversation( conversationParameters );
        const conversationReference = TurnContext.getConversationReference( context.activity ) as ConversationReference;
        conversationReference.conversation.id = conversationResourceResponse.id;
        return [ conversationReference, conversationResourceResponse.activityId ];
    }
}
