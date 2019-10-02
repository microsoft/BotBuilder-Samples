// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot

// Import required packages
const path = require('path');
const restify = require('restify');
const { name } = require('./package.json');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, ConversationState, MemoryStorage } = require('botbuilder');
const { ActivityTypes } = require('botbuilder-core');

const { QnAMakerBot } = require('./bots/activelearning-bot');

// Note: Ensure you have a .env file and include all necessary credentials to access services like LUIS and QnAMaker.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create conversation and user state with in-memory storage provider.
const memoryStorage = new MemoryStorage();
const conversationState = new ConversationState(memoryStorage);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // Create a trace activity that contains the error object
    const traceActivity = {
        type: ActivityTypes.Trace,
        timestamp: new Date(),
        name: 'Turn Error',
        label: 'TurnError',
        value: `${ error }`,
        valueType: 'https://www.botframework.com/schemas/error'
    };
    // This check writes out errors to console log .vs. app insights.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);

    // Send a trace activity, which will be displayed in Bot Framework Emulator
    await context.sendActivity(traceActivity);

    // Send a message to the user
    await context.sendActivity(`Bot Framework encounted an error or bug in ${ name }.`);
    await context.sendActivity(`To continue to run this bot, please fix ${ name } source code.`);
};

// Create the QnAMakerBot.
let bot;
try {
    bot = new QnAMakerBot(conversationState);
} catch (err) {
    console.error(`[botInitializationError]: ${ err }`);
    process.exit();
}

// Create HTTP server.
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3999, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open qnamaker.bot file in the emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        await bot.onTurn(turnContext);
    });
});
