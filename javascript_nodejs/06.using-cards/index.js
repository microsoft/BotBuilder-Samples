// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BotFrameworkAdapter, MemoryStorage, ConversationState } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');
const path = require('path');
const restify = require('restify');
const { RichCardsBot } = require('./bot');

const CONFIG_ERROR = 1;

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({ path: ENV_FILE });

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open using-cards.bot file in the emulator.`);
});

// .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// Read configuration from .bot file.
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Bot name as defined in .bot file.
// See https://aka.ms/about-bot-file to learn more about .bot usage and configuration.
const BOT_CONFIGURATION = 'using-cards';

// Get endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about .bot files.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about bot state management.
// A bot requires a state store to persist its dialog and various states between messages.
const memoryStorage = new MemoryStorage();

// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
// For production bots use the Azure CosmosDB storage, or Azure Blob storage providers.
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // This is the name of the Cosmos DB configuration in your .bot file.
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString,
//                                            authKey: ?,
//                                            databaseId: cosmosConfig.database,
//                                            collectionId: cosmosConfig.collection});

// Create conversation state with the in-memory storage provider.
const conversationState = new ConversationState(memoryStorage);

// Create a RichCardsBot which responds to a user with Rich Cards.
const bot = new RichCardsBot(conversationState);

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        // Route to bot.
        await bot.onTurn(turnContext);
    });
});
