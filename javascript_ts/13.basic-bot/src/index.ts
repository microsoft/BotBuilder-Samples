// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Generated using the Microsoft Botbuilder generator.
// See https://aka.ms/botbuildergenerator for more details.
//
// This bot was generated using the 'Basic' template.  This template uses
// the following capabilities:
//  An AI capable greeting using LUIS
//  A Getting Started card using an Adaptive Card
//  A multi-turn dialog interaction model
//
import * as path from 'path';
import * as restify from 'restify';
import { BotFrameworkAdapter, BotStateSet,  MemoryStorage, ConversationState, UserState, TurnContext } from 'botbuilder';
import { BotConfiguration, IEndpointService } from 'botframework-config';
import { Bot } from './bot';
import { config } from 'dotenv';

const BOT_CONFIGURATION = (process.env.NODE_ENV || 'development');
const BOT_CONFIGURATION_ERROR = 1;

// Create server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet the Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open the basic-bot.bot file in the Emulator`);
});

// Read botFilePath and botFileSecret from .env file.
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = config({path: ENV_FILE});

// .bot file path
const BOT_FILE_PATH = path.join(__dirname, '..', (process.env.botFilePath || ''));
let botConfig: BotConfiguration;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE_PATH, process.env.botFileSecret);
} catch(err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Get bot endpoint configuration by service name
let endpointConfig: IEndpointService;
endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create the adapter
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Setup our global error handler
//
// For production bots use AppInsights, or a production-grade telemetry service to
// log errors and other bot telemetry.
// const { TelemetryClient } = require("applicationinsights");
// Get AppInsights configuration by service name
// const APPINSIGHTS_CONFIGURATION = 'appInsights';
// const appInsightsConfig = botConfig.findServiceByNameOrId(APPINSIGHTS_CONFIGURATION);
// const telemetryClient = new TelemetryClient(appInsightsConfig.instrumentationKey);

// adapter.onTurnError(async (turnContext, error) => {
//     // CAUTION:  The sample simply logs the error to the console.
//     console.error(error);
//     // For production bots, use AppInsights or similar telemetry system.

//     // tell the user something happen

//     // for multi-turn dialog interactions,
//     // make sure we clear the conversation state
// });

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

// Use the BotStateSet middleware to automatically read and write conversation and user state.
// CONSIDER:  if only using userState, then switch to adapter.use(userState);
adapter.use(new BotStateSet(conversationState, userState));

let myBot: Bot;
try {
    myBot = new Bot(conversationState, userState, botConfig)
} catch (err) {
    console.log(`Error: ${err}`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Listen for incoming requests
server.post('/api/messages', (req, res) => {
    // Route received a request to adapter for processing
    adapter.processActivity(req, res, async (turnContext: TurnContext) => {
        // route to bot activity handler.
        await myBot.onTurn(turnContext);
    });
});