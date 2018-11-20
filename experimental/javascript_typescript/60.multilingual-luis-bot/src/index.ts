// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as path from 'path';
import * as restify from 'restify';
import { BotFrameworkAdapter, ConversationState, MemoryStorage, UserState } from 'botbuilder';
import { BotConfiguration } from 'botframework-config';
import { TranslatorMiddleware } from './translator-middleware';
import { LuisBot } from './bot';

// Used to create the BotStatePropertyAccessor for storing the user's language preference.
const LANGUAGE_PREFERENCE = 'language_preference';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({ path: ENV_FILE });

// Get the .bot file path.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));
let botConfig;
try {
    // Read bot configuration from .bot file.
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.error(`\nError reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    console.error(`\n - The botFileSecret is available under appsettings for your Azure Bot Service bot.`);
    console.error(`\n - If you are running this bot locally, consider adding a .env file with botFilePath and botFileSecret.`);
    console.error(`\n - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.\n\n`);
    process.exit();
}

// For local development configuration as defined in .bot file.
const DEV_ENVIRONMENT = 'development';

// Bot name as defined in .bot file or from runtime.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = (process.env.NODE_ENV || DEV_ENVIRONMENT);

// Language Understanding (LUIS) service name as defined in the .bot file.
const LUIS_CONFIGURATION = (process.env.LUIS_ENVIRONMENT || DEV_ENVIRONMENT);;

// Get bot endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);

// Map the contents to the required format for `LuisRecognizer`.
const luisApplication = {
    applicationId: luisConfig.appId,
    endpoint: luisConfig.endpoint,
    endpointKey: luisConfig.subscriptionKey
}

// Create configuration for LuisRecognizer's runtime behavior.
const luisPredictionOptions = {
    includeAllIntents: true,
    log: true,
    staging: false
};

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// // Catch-all for errors.
adapter.onTurnError = async (turnContext, error) => {
    // This check writes out errors to console log.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user.
    await turnContext.sendActivity(`Oops. Something went wrong!`);

//     // Load and then clear out state. This is to prevent the user from being stuck
//     // in a conversation due to bad state.
    await conversationState.load(turnContext);
    conversationState.clear(turnContext);
    await conversationState.saveChanges(turnContext);
};

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state store to persist the dialog and user state between messages.
let conversationState, userState;
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, everything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
conversationState = new ConversationState(memoryStorage);

// We use the UserState to track the user's language preference for translation.
userState = new UserState(memoryStorage);

// CAUTION: You must ensure your product environment has the NODE_ENV set
//          to use the Azure Blob storage or Azure Cosmos DB providers.

// Add botbuilder-azure when using any Azure services.
// const { BlobStorage } = require('botbuilder-azure');
// // Get service configuration
// const blobStorageConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION_ID);
// const blobStorage = new BlobStorage({
//     containerName: (blobStorageConfig.container || DEFAULT_BOT_CONTAINER),
//     storageAccountOrConnectionString: blobStorageConfig.connectionString,
// });
// conversationState = new ConversationState(blobStorage);
// userState = new UserState(blobStorage);

var noTranslatePatterns = {
    "fr":[
        "près de (.+)",
        "revues du (.+)",
        "mon nom est (.+)"
    ]
};

var customLanguageDictionary = {
    "Bonjour": "Greetings"
};

adapter.use(new TranslatorMiddleware("en", process.env.translatorKey, noTranslatePatterns, customLanguageDictionary));

// Create the LuisBot.
let bot;
try {
    bot = new LuisBot(luisApplication, luisPredictionOptions);
} catch (err) {
    console.error(`[botInitializationError]: ${ err }`);
    process.exit();
}

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open multilingual.bot file in the Emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        await bot.onTurn(turnContext);
    });
});
