// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');

/**
 * Logs user and bot messages. It filters out ContinueConversation events coming from skill responses.
 */
class LoggerMiddleware {
    // This defaults to using Node's console.log() method if a logger isn't passed in.
    constructor(logger = console) {
        this.logger = logger;
    }

    async onTurn(turnContext, next) {
        // Note: Skill responses will show as ContinueConversation events; we don't log those.
        // We only log incoming messages from users.
        if (turnContext.activity.type === ActivityTypes.Event && turnContext.activity.name !== 'continueConversation') {
            const message = `User said: "${ turnContext.activity.text }" Type: "${ turnContext.activity.type }" Name: "${ turnContext.activity.name }"`;
            this.logger.log(message);
        }

        // Register outgoing handler.
        turnContext.onSendActivities(this.outgoingHandler.bind(this));

        // Continue processing messages.
        await next();
    }

    async outgoingHandler(turnContext, activities, next) {
        activities.forEach((activity) => {
            const message = `Bot said: "${ activity.text }" Type: "${ activity.type }" Name: "${ activity.name }"`;
            this.logger.log(message);
        });

        await next();
    }
}

module.exports.LoggerMiddleware = LoggerMiddleware;