// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityTypes, TurnContext } from 'botbuilder';

/**
 * Simple bot that echoes received messages.
 */
export class ConsoleEchoBot {

    /**
     * Driver code for the bot. This bot only responds to "Message"-type
     * Activities. If the user's message is "quit", the process will exit.
<<<<<<< HEAD
     * @param turnContext Context for the current turn of conversation with the user.
     */
    async onTurn(turnContext: TurnContext) {
=======
     * @param {TurnContext} context on turn context object.
     */
    public onTurn = async (turnContext: TurnContext) => {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        // Only respond to "Message"-type Activities.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // If the user sent a simple "quit" message, close the Node.js process.
            if (turnContext.activity.text.toLowerCase() === 'quit') {
                process.exit();
<<<<<<< HEAD
            
=======

>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            // Otherwise echo back to the user the received message.
            } else if (turnContext.activity.text) {
                await turnContext.sendActivity(`You sent '${ turnContext.activity.text }'`);
            }
        }
    }
}
