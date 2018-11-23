// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TurnContext } from 'botbuilder';

export class MyBot {
    /**
     * Use onTurn to handle an incoming activity, received from a user, process it, and reply as needed
     *
     * @param {TurnContext} turnContext context object.
     */
    public onTurn = async (turnContext: TurnContext) => {
        // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
        await turnContext.sendActivity(`[${ turnContext.activity.type } event detected]`);
    }
}
