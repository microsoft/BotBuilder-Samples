// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

<<<<<<< HEAD
import * as path from 'path';
import * as restify from 'restify';
import { BotFrameworkAdapter } from 'botbuilder';
import { LuisApplication, LuisPredictionOptions } from 'botbuilder-ai';
import { BotConfiguration, IEndpointService, ILuisService } from 'botframework-config';
=======
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';

import { BotConfiguration, IEndpointService, LuisService } from 'botframework-config';

import { BotFrameworkAdapter } from 'botbuilder';
import { LuisApplication, LuisPredictionOptions } from 'botbuilder-ai';
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
import { LuisBot } from './bot';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
<<<<<<< HEAD
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({path: ENV_FILE});

// Get the .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));
let botConfig: BotConfiguration;
try {
    // Read bot configuration from .bot file. 
=======
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = config({path: ENV_FILE});

// Get the .bot file path.
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
const BOT_FILE = path.join(__dirname, '..', (process.env.botFilePath || ''));
let botConfig: BotConfiguration;
try {
    // Read bot configuration from .bot file.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.error(`\nError reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    console.error(`\n - The botFileSecret is available under appsettings for your Azure Bot Service bot.`);
    console.error(`\n - If you are running this bot locally, consider adding a .env file with botFilePath and botFileSecret.\n\n`);
<<<<<<< HEAD
=======
    console.error(`\n - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.\n\n`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    process.exit();
}
// For local development configuration as defined in .bot file.
const DEV_ENVIRONMENT = 'development';

// Bot name as defined in .bot file or from runtime.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = (process.env.NODE_ENV || DEV_ENVIRONMENT);

// Language Understanding (LUIS) service name as defined in the .bot file.
const LUIS_CONFIGURATION = '';

<<<<<<< HEAD
// Get endpoint and LUIS configurations by service name.
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const luisConfig = <ILuisService>botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
=======
if (!LUIS_CONFIGURATION) {
    console.error('Make sure to update the index.ts file with a LUIS_CONFIGURATION name that matches your .bot file.');
    process.exit();
}

// Get endpoint and LUIS configurations by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION) as IEndpointService;
const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION) as LuisService;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

// Map the contents to the required format for `LuisRecognizer`.
const luisApplication: LuisApplication = {
    applicationId: luisConfig.appId,
    endpoint: luisConfig.getEndpoint(),
<<<<<<< HEAD
    endpointKey: luisConfig.subscriptionKey
}
=======
    endpointKey: luisConfig.subscriptionKey,
};
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

// Create configuration for LuisRecognizer's runtime behavior.
const luisPredictionOptions: LuisPredictionOptions = {
    includeAllIntents: true,
    log: true,
<<<<<<< HEAD
    staging: false
}
=======
    staging: false,
};
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
<<<<<<< HEAD
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (turnContext, error) => {
    console.error(`\n [onTurnError]: ${ error }`);
    await turnContext.sendActivity(`Oops. Something went wrong!`);
=======
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword,
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    console.error(`\n [onTurnError]: ${ error }`);
    await context.sendActivity(`Oops. Something went wrong!`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
};

// Create the LuisBot.
let bot: LuisBot;
try {
    bot = new LuisBot(luisApplication, luisPredictionOptions);
} catch (err) {
    console.error(`[botInitializationError]: ${ err }`);
    process.exit();
}

// Create HTTP server
<<<<<<< HEAD
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
=======
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open nlp-with-luis.bot file in the emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (turnContext) => {
        await bot.onTurn(turnContext);
    });
});
