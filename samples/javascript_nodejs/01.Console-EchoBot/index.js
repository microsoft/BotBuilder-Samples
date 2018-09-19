// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const { ConsoleAdapter } = require('./consoleAdapter');

// load environment variables from .env file.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create the bot adapter, which is responsible for sending and receiving messages.
// We are using the ConsoleAdapter, which enables a bot you can chat with from within your terminal window.
const adapter = new ConsoleAdapter();

// A call to adapter.listen tells the adapter to start listening for incoming messages and events, known as "activities."
// Activities are received as TurnContext objects by the handler function.
adapter.listen(async (context) => {
    // Check to see if this activity is an incoming message.
    // (It could theoretically be another type of activity.)
    if (context.activity.type === 'message') {
        // Check to see if the user sent a simple "quit" message.
        if (context.activity.text.toLowerCase() === 'quit') {
            // Send a reply.
            context.sendActivity(`Bye!`);
            process.exit();
        } else if (context.activity.text) {
            // Echo the message text back to the user.
            return context.sendActivity(`I heard you say "${ context.activity.text }"`);
        }
    }
});

// Emit a startup message with some instructions.
console.log('> Console EchoBot is online. I will repeat any message you send me!');
console.log('> Say "quit" to end.');
console.log(''); // Leave a blank line after instructions.
