// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ActivityHandler,
    MessageFactory
} = require('botbuilder');

class MessageReactionBot extends ActivityHandler {
    constructor(activityLog) {
        super();
        this._log = activityLog;
        this.onMessage(async (context, next) => {
            await this.sendMessageAndLogActivityId(context, `echo: ${ context.activity.text }`);
            await next();
        });
    }

    async onReactionsAddedActivity(reactionsAdded, context) {
        for (var i = 0, len = reactionsAdded.length; i < len; i++) {
            var activity = await this._log.find(context.activity.replyToId);
            if (!activity) {
                // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                await this.sendMessageAndLogActivityId(context, `Activity ${ context.activity.replyToId } not found in the log.`);
            } else {
                await this.sendMessageAndLogActivityId(context, ` added '${ reactionsAdded[i].type }' regarding '${ activity.text }'`);
            }
        };
    }

    async onReactionsRemovedActivity(reactionsAdded, context) {
        for (var i = 0, len = reactionsAdded.length; i < len; i++) {
            // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity that was previously sent from this bot.
            var activity = await this._log.find(context.activity.replyToId);
            if (!activity) {
                // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                await this.sendMessageAndLogActivityId(context, `Activity ${ context.activity.replyToId } not found in the log.`);
            } else {
                await this.sendMessageAndLogActivityId(context, `You removed '${ reactionsAdded[i].type }' regarding '${ activity.text }'`);
            }
        };
    }

    async sendMessageAndLogActivityId(context, text) {
        const replyActivity = MessageFactory.text(text);
        const resourceResponse = await context.sendActivity(replyActivity);
        await this._log.append(resourceResponse.id, replyActivity);
    }
}

module.exports.MessageReactionBot = MessageReactionBot;
