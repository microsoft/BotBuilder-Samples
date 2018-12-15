// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Generated using the Microsoft Botbuilder generator.
// See https://aka.ms/botbuildergenerator for more details.

const path = require('path');
const restify = require('restify');
require('dotenv').config({ path: path.join(__dirname, '.env') });

const { BotFrameworkAdapter, MemoryStorage, ConversationState, UserState } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');

const MessageRoutingBot = require('./bot');
const BOT_CONFIGURATION = (process.env.NODE_ENV || 'development');
const BOT_CONFIGURATION_ERROR = 1;

// Create server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet the Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open the message-routing.bot file in the Emulator`);
});

// read bot configuration from .bot file.
// See https://aka.ms/about-bot-file to learn more about bot file its use.
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(path.join(__dirname, process.env.botFilePath), process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Get bot endpoint configuration by service name
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create the adapter
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    await context.sendActivity(`Oops. Something went wrong!`);
    // Clear out state
    await conversationState.delete(context);
};

// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
// For production bots use the Azure CosmosDB storage, Azure Blob, or Azure Table storage provides.
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // this is the name of the cosmos DB configuration in your .bot file
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString,
//                                            authKey: ?,
//                                            databaseId: cosmosConfig.database,
//                                            collectionId: cosmosConfig.collection});

// create conversation and user state with in-memory storage provider.
const conversationState = new ConversationState(memoryStorage);
const userState = new UserState(memoryStorage);

// Create main dialog.
let bot;
try {
    bot = new MessageRoutingBot(conversationState, userState, botConfig);
} catch (err) {
    console.log(`Error: ${ err }`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Listen for incoming requests
server.post('/api/messages', (req, res) => {
    // Route received a request to adapter for processing
    adapter.processActivity(req, res, async (turnContext) => {
        // route to bot activity handler.
        await bot.onTurn(turnContext);
    });
});
