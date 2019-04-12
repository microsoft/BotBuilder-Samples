// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, TurnContext } = require('botbuilder');

class ProactiveBot extends ActivityHandler {
    constructor(conversationReferences) {
        super();

        this.conversationReferences = conversationReferences;

        this.onConversationUpdate(async (context, next) => {

            const reference = TurnContext.getConversationReference(context.activity);
            this.conversationReferences[reference.conversation.id] = reference;
            
            await next();
        });
    }
}

module.exports.ProactiveBot = ProactiveBot;
