// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotFrameworkAdapter } from 'botbuilder';
import { LuisApplication, LuisPredictionOptions } from 'botbuilder-ai';
import { BotConfiguration, IEndpointService, ILuisService } from 'botframework-config';
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';
import { LuisBot } from './bot';

const CONFIG_ERROR = 1;

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({path: ENV_FILE});

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open nlp-with-luis.bot file in the emulator.`);
});

// .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// Read bot configuration from .bot file. 
let botConfig: BotConfiguration;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Bot name and Language Understanding (LUIS) service name as defined in the .bot file.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = 'nlp-with-luis';
const LUIS_CONFIGURATION = '';

// Get endpoint and LUIS configurations by service name.
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const luisConfig = <ILuisService>botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);

// Map the contents to the required format for `LuisRecognizer`.
const luisApplication: LuisApplication = {
    applicationId: luisConfig.appId,
    azureRegion: luisConfig.region,
    endpointKey: luisConfig.subscriptionKey
}

// Create configuration for LuisRecognizer's runtime behavior.
const luisPredictionOptions: LuisPredictionOptions = {
    includeAllIntents: true,
    log: true,
    staging: false
}

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Create the bot that handles incoming Activities.
const luisBot: LuisBot = new LuisBot(luisApplication, luisPredictionOptions);

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (turnContext) => {
        await luisBot.onTurn(turnContext);        
    });
});
