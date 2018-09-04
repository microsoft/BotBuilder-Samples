// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const restify = require('restify');
const path = require('path');
const ERROR = 1;

// Import required bot services. See https://aka.ms/bot-services to learn more about the different part of a bot
const { BotFrameworkAdapter, BotStateSet,  MemoryStorage, ConversationState, UserState } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');

const MainDialog = require('./dialogs/mainDialog');

// Read botFilePath and botFileSecret from .env file
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({path: ENV_FILE});

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open prompt-validations-bot.bot file in the Emulator`);
});

// .bot file path
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

console.log('reading config from ', BOT_FILE);
// read bot configuration from .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment`);
    console.log(err);
    process.exit(ERROR);
}

// bot name as defined in .bot file 
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration .
const BOT_CONFIGURATION = 'prompt-validations-bot';

// Get bot endpoint configuration by service name
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about .bot file its use and bot configuration .
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Define state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a some sort of state storage system to persist the dialog and user state between messages.
const memoryStorage = new MemoryStorage();
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone. 
// For production bots use the Azure CosmosDB storage, or Azure Blob storage providers. 
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // this is the name of the CosmosDB configuration in your .bot file
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString, 
//                                            authKey: ?, 
//                                            databaseId: cosmosConfig.database, 
//                                            collectionId: cosmosConfig.collection});

// Create conversation state with in-memory storage provider. 
const conversationState = new ConversationState(memoryStorage);
const userState = new UserState(memoryStorage)

// Use the BotStateSet middleware to automatically read and write conversation and user state.
adapter.use(new BotStateSet(conversationState, userState));

// Create the main dialog.
const mainDlg = new MainDialog(conversationState, userState);

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // route to main dialog.
        await mainDlg.onTurn(context);        
    });
});
