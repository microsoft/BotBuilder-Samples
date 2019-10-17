// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const path = require('path');
const { ConsoleAdapter } = require('./consoleAdapter');

// load environment variables from .env file.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create the bot adapter, which is responsible for sending and receiving messages.
// We are using the ConsoleAdapter, which enables a bot you can chat with from within your terminal window.
const adapter = new ConsoleAdapter();

// Import our bot class.
const { EchoBot } = require('./bot');
const bot = new EchoBot();

// A call to adapter.listen tells the adapter to start listening for incoming messages and events, known as "activities."
// Activities are received as TurnContext objects by the handler function.
adapter.listen(async (context) => {
    bot.onTurn(context);
});

// Emit a startup message with some instructions.
console.log('> Console EchoBot is online. I will repeat any message you send me!');
console.log('> Say "quit" to end.');
console.log(''); // Leave a blank line after instructions.
