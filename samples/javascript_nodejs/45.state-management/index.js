// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, MemoryStorage, ConversationState, UserState } = require('botbuilder');

// This bot's main dialog.
const { StateManagementBot } = require('./bots/stateManagementBot');

// Read environment variables from .env file
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create HTTP server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
});

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Define state store for your bot.
// See https://aka.ms/about-bot-state to learn more about bot state.
const memoryStorage = new MemoryStorage();

// Create conversation and user state with in-memory storage provider.
const conversationState = new ConversationState(memoryStorage);
const userState = new UserState(memoryStorage);

// Create the bot.
const bot = new StateManagementBot(conversationState, userState);

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    // Send a message to the user
    const onTurnErrorMessage = `Sorry, it looks like something went wrong!`;
    await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage);
    // Clear out state
    await conversationState.delete(context);
};

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // Route to main dialog.
        await bot.run(context);
    });
});
