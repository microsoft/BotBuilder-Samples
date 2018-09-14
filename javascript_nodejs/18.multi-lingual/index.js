// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BotFrameworkAdapter, ConversationState, MemoryStorage, UserState } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');
const MultilingualBot = require('./bot');
const TranslatorMiddleware = require('./translator-middleware');
const path = require('path');
const restify = require('restify');

const CONFIG_ERROR = 1;
const LANGUAGE_PREFERENCE = 'language_preference';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({ path: ENV_FILE });

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${server.name} listening to ${server.url}.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open multilingual.bot file in the Emulator.`);
});

// .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// Read bot configuration from .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Bot name as defined in .bot file. 
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = 'multilingual';

// Get bot endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// Define the state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state storage system to persist the dialog and user state between messages.
const memoryStorage = new MemoryStorage();

// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, everything stored in memory will be gone. 
// For production bots use the Azure Cosmos DB storage, or Azure Blob storage providers. 
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // Cosmos DB configuration in your .bot file
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString, 
//                                            authKey: ?, 
//                                            databaseId: cosmosConfig.database, 
//                                            collectionId: cosmosConfig.collection});

// Create conversation state with in-memory storage provider. 
const conversationState = new ConversationState(memoryStorage);

// We use the UserState to track the user's language preference for translation.
const userState = new UserState(memoryStorage);
const languagePreferenceProperty = userState.createProperty(LANGUAGE_PREFERENCE);

adapter.use(new TranslatorMiddleware(languagePreferenceProperty, process.env.translatorKey));

// Create the bot that will handle incoming messages.
const multilingualBot = new MultilingualBot(userState, languagePreferenceProperty);

// Listen for incoming requests. 
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async(context) => {
        await multilingualBot.onTurn(context);
    });
});