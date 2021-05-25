// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConsoleAdapter} from './consoleAdapter';

import { ConsoleEchoBot } from './bot';

import { TurnContext } from 'botbuilder';

// Create the adapter which is responsible for sending and receiving messages.
// The ConsoleAdapter enables a user to chat with a bot from within their console window.
const adapter: ConsoleAdapter = new ConsoleAdapter();

// Create the bot that processes incoming Activities via its onTurn() method.
const echoBot: ConsoleEchoBot = new ConsoleEchoBot();

// `adapter.listen` tells the adapter to listen for incoming messages
// and events, known as "Activities."
// Activities are wrapped in TurnContext objects by the handler function.
const closeFn = adapter.listen(async (turnContext: TurnContext) => {
    await echoBot.onTurn(turnContext);
});

// Emit a startup message with some instructions.
console.log('> Console EchoBot is online. I will repeat any message you send me!');
console.log('> Say "quit" to end.\n');
