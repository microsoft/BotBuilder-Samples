// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as restify from 'restify';
import * as path from 'path';
import { config } from 'dotenv';
import { BotFrameworkAdapter, MemoryStorage, ConversationState } from 'botbuilder';
import { BotConfiguration, IEndpointService } from 'botframework-config';
import { MainDialog } from './dialogs/mainDialog';

// bot name as defined in .bot file 
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration .
const BOT_CONFIGURATION = 'echobot-with-counter';

// read botFilePath and botFileSecret from .env file
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = config({path: ENV_FILE});

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet emulator: https://aka.ms/botframework-emulator`);
    // TODO: Add emulator deep link
    console.log(`\nTo talk to your bot, open the EchoBot-With-Counter.bot file in the Emulator`);
});

// .bot file path
const BOT_FILE = path.join(__dirname, '..', process.env.botFilePath);
// read bot configuration from .bot file.
const botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
// Get bot endpoint configuration by service name
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about .bot file its use and bot configuration .
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Define state store for your bot. See https://aka.ms/about-bot-state to learn more about bots memory service
// A bot requires a sate store to priciest it dialog and user state between messages
const memoryStorage = new MemoryStorage();
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone. 
// For production bots use the Azure CosmosDB storage, Azure Blob, or Azure Table storage provides. 
// const { CosmosDbStorage } = require('botbuilder-azure');
// const STORAGE_CONFIGURATION = 'cosmosDB'; // this is the name of the cosmos DB configuration in your .bot file
// const cosmosConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION);
// const cosmosStorage = new CosmosDbStorage({serviceEndpoint: cosmosConfig.connectionString, 
//                                            authKey: ?, 
//                                            databaseId: cosmosConfig.database, 
//                                            collectionId: cosmosConfig.collection});

// create conversation state with in-memory storage provider. 
const convoState = new ConversationState(memoryStorage);

// register conversation state as a middleware. The ConversationState middleware automatically reads and writes conversation sate 
adapter.use(convoState);

// Create the main dialog.
const mainDlg = new MainDialog(convoState);

// Listen for incoming requests 
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // route to main dialog.
        await mainDlg.onTurn(context);        
    });
});

