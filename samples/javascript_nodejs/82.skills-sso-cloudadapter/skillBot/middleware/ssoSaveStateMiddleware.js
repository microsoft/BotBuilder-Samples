// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');

class SsoSaveStateMiddleware {
    /**
     * @param {import('botbuilder').ConversationState} conversationState
     */
    constructor(conversationState) {
        this.conversationState = conversationState;
    }

    /**
     * @param {import('botbuilder').TurnContext} turnContext
     */
    async onTurn(turnContext, next) {
        // Register outgoing handler.
        turnContext.onSendActivities(this.outgoingHandler.bind(this));

        // Continue processing messages.
        await next();
    }

    /**
     * @param {import('botbuilder').TurnContext} turnContext
     * @param {Partial<import('botbuilder').Activity>[]} activities
     * @param {Function} next
     */
    async outgoingHandler(turnContext, activities, next) {
        for (const activity of activities) {
            if (!!activity.attachments && activity.attachments.some(attachment => attachment.contentType === CardFactory.contentTypes.oauthCard)) {
                this.conversationState.saveChanges(turnContext);
            }
        }

        return next();
    }
}

module.exports.SsoSaveStateMiddleware = SsoSaveStateMiddleware;
