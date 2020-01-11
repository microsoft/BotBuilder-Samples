// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

class EchoBot {
    async onTurn(context) {
        // Check to see if this activity is an incoming message.
        // (It could theoretically be another type of activity.)
        if (context.activity.type === 'message' && context.activity.text) {
            // Check to see if the user sent a simple "quit" message.
            if (context.activity.text.toLowerCase() === 'quit') {
                // Send a reply.
                context.sendActivity('Bye!');
                process.exit();
            } else {
                // Echo the message text back to the user.
                return context.sendActivity(`I heard you say "${ context.activity.text }"`);
            }
        }
    }
}

module.exports = { EchoBot };
