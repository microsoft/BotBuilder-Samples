// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BotFrameworkAdapter, ConversationState, MemoryStorage } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');
const restify = require('restify');
const path = require('path');
const { GraphAuthenticationBot } = require('./bot');

const CONFIG_ERROR = 1;

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({ path: ENV_FILE });

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${server.name} listening to ${server.url}.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open authentication-msgraph.bot file in the Emulator.`);
});

// The .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// Read the bot's configuration from the .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Bot name as defined in the .bot file.
const BOT_CONFIGURATION = '';

// Get bot endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create the adapter. See https://aka.ms/about-bot-adapter to learn more adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppID,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

adapter.onTurnError = async(turnContext, error) => {
    console.error(error);
}

// Define state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// Bots require a state storage system to persist the dialog and user state between messages.
const memoryStorage = new MemoryStorage();

// CAUTION: The MemoryStorage used here is for local bot debugging only.
// After restarting, anything stored in MemoryStorage is erased. 
// For production bots, use the Azure CosmosDB storage or Azure Blob storage providers.
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // this is the name of the CosmosDB configuration in your .bot file
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString, 
//                                            authKey: ?, 
//                                            databaseId: cosmosConfig.database, 
//                                            collectionId: cosmosConfig.collection});

// Create conversation state with in-memory storage provider. 
const conversationState = new ConversationState(memoryStorage);

// Create the GraphAuthenticationBot.
const graphAuthenticationBot = new GraphAuthenticationBot(conversationState);

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async(turnContext) => {
        await graphAuthenticationBot.onTurn(turnContext);
    });
});