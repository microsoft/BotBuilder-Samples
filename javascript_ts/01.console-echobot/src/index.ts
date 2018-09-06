// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ConsoleAdapter, TurnContext } from 'botbuilder';
import { ConsoleEchoBot } from './bot';

// Create the bot adapter, which is responsible for sending and receiving messages.
// The ConsoleAdapter enables a user to chat with a bot from within their terminal window.
const adapter = new ConsoleAdapter();

// Create the ConsoleEchoBot that will process incoming Activities via its onTurn() method.
const echoBot: ConsoleEchoBot = new ConsoleEchoBot();

// `adapter.listen` tells the adapter to start listening for incoming messages
// and events, known as "Activities."
// Activities are wrapped in TurnContext objects by the handler function.
const closeFn = adapter.listen(async (turnContext: TurnContext) => {
    await echoBot.onTurn(turnContext);
});

// Emit a startup message with some instructions.
console.log('> Console EchoBot is online. I will repeat any message you send me!');
console.log('> Say "quit" to end.\n');