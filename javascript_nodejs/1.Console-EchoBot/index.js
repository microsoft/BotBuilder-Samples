// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const botbuilder = require('botbuilder');
const path = require('path');
const config = require('dotenv').config;

const mainDialog = require('./dialogs/mainDialog');

// load environment variables from .env file.
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = config({path: ENV_FILE});

// create the Bot adapter, which is responsible for sending and receiving messages
// in this case, we are using the ConsoleAdapter, which enables a bot you can chat with from within your terminal window.
const adapter = new botbuilder.ConsoleAdapter();

// tell the adapter to start listening for incoming messages and events, known as "activities"
// activities arrive as TurnContext objects
const closeFn = adapter.listen(async (context) => {
    await mainDialog(context);
});

// emit a startup message with some instructions.
console.log('> Console EchoBot is online. I will repeat any message you send me!');
console.log('> Say "quit" to end.');
console.log(''); // leave a blank line after instructions