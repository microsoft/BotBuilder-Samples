// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    MessageFactory,
    MessageReaction,
    ActivityHandler,
    TurnContext,
} from 'botbuilder';

import {
    ActivityLog
} from './activityLog';

export class MessageReactionBot extends ActivityHandler {
    _log: ActivityLog;

    /*
     * From the UI you need to @mention the bot, then add a message reaction to the message the bot sent.
     */
    constructor(activityLog: ActivityLog) {
        super();

        this._log = activityLog;

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await this.sendMessageAndLogActivityId(context, `echo: ${context.activity.text}`);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    protected async onReactionsAddedActivity(reactionsAdded: MessageReaction[], context: TurnContext): Promise<void> {
        for (var i = 0, len = reactionsAdded.length; i < len; i++) {
            var activity = await this._log.find(context.activity.replyToId);
            if (activity == null) {
                // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                await this.sendMessageAndLogActivityId(context, `Activity ${context.activity.replyToId} not found in the log.`);
            }
            else {
                await this.sendMessageAndLogActivityId(context, `You added '${reactionsAdded[i].type}' regarding '${activity.text}'`);
            }
        };

        return;
    }

    protected async onReactionsRemovedActivity(reactionsAdded: MessageReaction[], context: TurnContext): Promise<void> {
        for (var i = 0, len = reactionsAdded.length; i < len; i++) {
            // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity that was previously sent from this bot.
            var activity = await this._log.find(context.activity.replyToId);
            if (activity == null) {
                // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                await this.sendMessageAndLogActivityId(context, `Activity ${context.activity.replyToId} not found in the log.`);
            }
            else {
                await this.sendMessageAndLogActivityId(context, `You removed '${reactionsAdded[i].type}' regarding '${activity.text}'`);
            }
        };

        return;
    }

    async sendMessageAndLogActivityId(context: TurnContext, text: string): Promise<void> {
        var replyActivity = MessageFactory.text(text);
        var resourceResponse = await context.sendActivity(replyActivity);
        await this._log.append(resourceResponse.id, replyActivity);

        return;
    }
}
