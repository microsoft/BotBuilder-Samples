// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class MyBot {
    /**
     *
     * @param {TurnContext} turnContext object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        await turnContext.sendActivity(`[${ turnContext.activity.type } event detected]`);
    }
}

module.exports.MyBot = MyBot;
