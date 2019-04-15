// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, TurnContext } = require('botbuilder');

class ProactiveBot extends ActivityHandler {
    constructor(conversationReferences) {
        super();

        this.conversationReferences = conversationReferences;

        this.onConversationUpdate(async (context, next) => {

            const conversationReference = TurnContext.getConversationReference(context.activity);
            this.conversationReferences[conversationReference.conversation.id] = conversationReference;
            
            await next();
        });
    }
}

module.exports.ProactiveBot = ProactiveBot;
