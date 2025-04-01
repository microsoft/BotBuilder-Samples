// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const { CardFactory } = require('botbuilder');

/** @import { Activity, ConversationState, TurnContext } from 'botbuilder' */

class SsoSaveStateMiddleware {
    /**
     * @param {ConversationState} conversationState
     */
    constructor(conversationState) {
        this.conversationState = conversationState;
    }

    /**
     * @param {TurnContext} turnContext
     */
    async onTurn(turnContext, next) {
        // Register outgoing handler.
        turnContext.onSendActivities(this.outgoingHandler.bind(this));

        // Continue processing messages.
        await next();
    }

    /**
     * @param {TurnContext} turnContext
     * @param {Partial<Activity>[]} activities
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
