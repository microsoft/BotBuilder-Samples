"use strict";
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const restify = require("restify");
const path = require("path");
const dotenv_1 = require("dotenv");
const botbuilder_1 = require("botbuilder");
const botframework_config_1 = require("botframework-config");
const mainDialog_1 = require("./dialogs/mainDialog");
// bot name as defined in .bot file 
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration .
const BOT_CONFIGURATION = 'echobot-with-counter';
// read botFilePath and botFileSecret from .env file
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = dotenv_1.config({ path: ENV_FILE });
// .bot file path
const BOT_FILE = path.join(__dirname, '..', process.env.botFilePath);
// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open echobot-with-counter.bot file in the Emulator.`);
});
// read bot configuration from .bot file.
const botConfig = botframework_config_1.BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
// Get bot endpoint configuration by service name
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
// Create adapter. See https://aka.ms/about-bot-adapter to learn more about .bot file its use and bot configuration .
const adapter = new botbuilder_1.BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});
// Define state store for your bot. See https://aka.ms/about-bot-state to learn more about bots memory service
// A bot requires a sate store to priciest it dialog and user state between messages
const memoryStorage = new botbuilder_1.MemoryStorage();
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
const conversationState = new botbuilder_1.ConversationState(memoryStorage);
// register conversation state as a middleware. The ConversationState middleware automatically reads and writes conversation sate 
adapter.use(conversationState);
// Create the main dialog.
const mainDlg = new mainDialog_1.MainDialog(conversationState);
// Listen for incoming requests 
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, (context) => __awaiter(this, void 0, void 0, function* () {
        // route to main dialog.
        yield mainDlg.onTurn(context);
    }));
});
//# sourceMappingURL=index.js.map