// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot

// Import required packages.
const path = require('path');
const restify = require('restify');
const { MicrosoftTranslator } = require('./translation/microsoftTranslator');
const { TranslatorMiddleware } = require('./translation/translatorMiddleware');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, MemoryStorage, UserState } = require('botbuilder');
const { ActivityTypes } = require('botbuilder-core');

// This bot's main dialog.
const { MultilingualBot } = require('./bots/multilingualBot');

// Note: Ensure you have a .env file and include translatorKey.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Used to create the BotStatePropertyAccessor for storing the user's language preference.
const LANGUAGE_PREFERENCE = 'language_preference';

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
        name: 'onTurnError Trace',
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
    await context.sendActivity(`The bot encounted an error or bug.`);
    await context.sendActivity(`To continue to run this bot, please fix the bot source code.`);
};

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state store to persist the dialog and user state between messages.

// For local development, in-memory storage is used.
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, everything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
const userState = new UserState(memoryStorage);

const languagePreferenceProperty = userState.createProperty(LANGUAGE_PREFERENCE);

const translator = new MicrosoftTranslator(process.env.translatorKey);
adapter.use(new TranslatorMiddleware(translator, languagePreferenceProperty));

// Create the MultilingualBot.
const bot = new MultilingualBot(userState, languagePreferenceProperty);

// Create HTTP server.
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
});

// Listen for incoming activities and route them to your bot main dialog.
server.post('/api/messages', (req, res) => {
    // Route received a request to adapter for processing.
    adapter.processActivity(req, res, async (context) => {
        // Route to bot activity handler.
        await bot.run(context);
    });
});
