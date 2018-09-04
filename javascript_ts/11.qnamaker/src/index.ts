// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotFrameworkAdapter, MemoryStorage, ConversationState } from 'botbuilder';
import { QnAMakerEndpoint } from 'botbuilder-ai';
import { BotConfiguration, IEndpointService, QnaMakerService, IQnAService, IAppInsightsService } from 'botframework-config';
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';
import { QnAMakerBot } from './bot';
import { MyAppInsightsMiddleware, MyAppInsightsMiddlewareSettings } from './middleware';

const CONFIG_ERROR = 1;

// bot name as defined in .bot file 
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration .
const BOT_CONFIGURATION = 'qnamaker';
const QNA_CONFIGURATION = 'qnamakerService';
const APP_INSIGHTS_CONFIGURATION = 'appInsights';

// Read botFilePath and botFileSecret from .env file
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({path: ENV_FILE});

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open qnamaker.bot file in the Emulator.`);
});

// .bot file path
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

let botConfig: BotConfiguration;
try {
    // Read bot configuration from .bot file. 
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment`);
    process.exit(CONFIG_ERROR);
}

// Get bot endpoint configuration by service name.
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const qnaConfig = <IQnAService>botConfig.findServiceByNameOrId(QNA_CONFIGURATION);
const appInsightsConfig = <IAppInsightsService>botConfig.findServiceByNameOrId(APP_INSIGHTS_CONFIGURATION);

// Map the contents of qnaConfig to a consumable format for our MyAppInsightsQnAMaker class.
const qnaEndpointSettings: QnAMakerEndpoint = {
    knowledgeBaseId: qnaConfig.kbId,
    endpointKey: qnaConfig.endpointKey,
    host: qnaConfig.hostname
}

// Create two variables to indicate whether or not the bot should include messages' text and usernames when
// sending information to Application Insights.
// These settings are used in the MyApplicationInsightsMiddleware and also in MyAppInsightsQnAMaker.
const logMessage = true;
const logName = true;

// Map the contents of appInsightsConfig to a consumable format for MyAppInsightsMiddleawre.
const appInsightsSettings: MyAppInsightsMiddlewareSettings = {
    instrumentationKey: appInsightsConfig.instrumentationKey,
    logOriginalMessage: logMessage,
    logUserName: logName
}

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about .bot file its use and bot configuration.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Register a MyAppInsightsMiddleware instance.
// This will send to Application Insights all incoming Message-type activities.
// It also stores in TurnContext.TurnState an `applicationinsights` TelemetryClient instance.
// This cached TelemetryClient is used by the custom class MyAppInsightsQnAMaker.
adapter.use(new MyAppInsightsMiddleware(appInsightsSettings));

// Create the main dialog.
const qnaMakerBot = new QnAMakerBot(qnaEndpointSettings, {}, logMessage, logName);

// Listen for incoming requests 
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (context) => {
        await qnaMakerBot.onTurn(context);        
    });
});
