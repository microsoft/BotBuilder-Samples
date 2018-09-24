// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as path from 'path';
import * as restify from 'restify';
import { BotFrameworkAdapter } from 'botbuilder';
import { QnAMakerEndpoint } from 'botbuilder-ai';
import { BotConfiguration, IEndpointService, IQnAService } from 'botframework-config';
import { QnAMakerBot } from './bot';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({path: ENV_FILE});

// Get the .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));
let botConfig: BotConfiguration;
try {
    // Read the configuration from .bot file. 
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.error(`\nError reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    console.error(`\n - The botFileSecret is available under appsettings for your Azure Bot Service bot.`);
    console.error(`\n - If you are running this bot locally, consider adding a .env file with botFilePath and botFileSecret.\n\n`);
    process.exit();
}

// For local development configuration as defined in .bot file.
const DEV_ENVIRONMENT = 'development';

// Bot name as defined in .bot file or from runtime.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = (process.env.NODE_ENV || DEV_ENVIRONMENT);
const QNA_CONFIGURATION = 'qnamakerService';

// Get bot endpoint and QnAMaker configuration by service name.
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const qnaConfig = <IQnAService>botConfig.findServiceByNameOrId(QNA_CONFIGURATION);

// Map the contents to the required format for QnAMaker.
const qnaEndpointSettings: QnAMakerEndpoint = {
    knowledgeBaseId: qnaConfig.kbId,
    endpointKey: qnaConfig.endpointKey,
    host: qnaConfig.hostname
}

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (turnContext, error) => {
    console.error(`\n [onTurnError]: ${ error }`);
    await turnContext.sendActivity(`Oops. Something went wrong!`);
};

// Create the QnAMakerBot.
let bot: QnAMakerBot;
try {
    bot = new QnAMakerBot(qnaEndpointSettings, {});
} catch (err) {
    console.error(`[botInitializationError]: ${ err }`);
    process.exit();
}

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open qnamaker.bot file in the emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (turnContext) => {
        await bot.onTurn(turnContext);        
    });
});
