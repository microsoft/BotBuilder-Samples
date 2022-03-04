// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    MessageFactory,
    TeamsActivityHandler,
    teamsGetChannelId,
    TeamsInfo
} = require('botbuilder');

class TeamsStartNewThreadInChannel extends TeamsActivityHandler {
    constructor() {
        super();

        this.onMessage(async (context, next) => {
            const teamsChannelId = teamsGetChannelId(context.activity);
            const activity = MessageFactory.text('This will be the first message in a new thread');
            const [reference] = await TeamsInfo.sendMessageToTeamsChannel(context, activity, teamsChannelId, process.env.MicrosoftAppId);

            await context.adapter.continueConversationAsync(process.env.MicrosoftAppId, reference, async turnContext => {
                await turnContext.sendActivity(MessageFactory.text('This will be the first response to the new thread'));
            });

            await next();
        });
    }
}

module.exports.TeamsStartNewThreadInChannel = TeamsStartNewThreadInChannel;
