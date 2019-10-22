// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ActivityHandler,
    ActivityTypes,
    TurnContext
} from 'botbuilder';

/*
 * From the UI you can just @mention the bot from any channelwith any string EXCEPT for "delete". If you send the bot "delete" it will delete
 * all of the previous bot responses and empty it's internal storage.
 */
export class ActivityUpdateAndDeleteBot extends ActivityHandler {
    protected activityIds: string[];

    constructor(activityIds: string[]) {
        super();

        this.activityIds = activityIds;

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {

            TurnContext.removeRecipientMention(context.activity);
            if (context.activity.text === 'delete') {
                for (const activityId of this.activityIds) {
                    await context.deleteActivity(activityId);
                }

                this.activityIds = [];
            } else {
                await this.sendMessageAndLogActivityId(context, `${context.activity.text}`);

                const text = context.activity.text;
                for (const id of this.activityIds) {
                    await context.updateActivity({ id, text, type: ActivityTypes.Message });
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    private async sendMessageAndLogActivityId(context: TurnContext, text: string): Promise<void> {
        const resourceResponse = await context.sendActivity({ text });
        await this.activityIds.push(resourceResponse.id);
    }
}
