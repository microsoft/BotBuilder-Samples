// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as restify from 'restify';
import * as teams from 'botbuilder-teams';
import { BotFrameworkAdapterSettings } from 'botbuilder-teams/node_modules/botbuilder';
import { FileBot } from './bot';
import * as path from 'path';
import { config } from 'dotenv';

// Read botFilePath and botFileSecret from .env file
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '..', '.env');
const loadFromEnv = config({path: ENV_FILE});

// // Create adapter. 
// See https://aka.ms/about-bot-adapter to learn more about to learn more about bot adapter.
const botSetting: Partial<BotFrameworkAdapterSettings> = {
  appId: process.env.microsoftAppID,
  appPassword: process.env.microsoftAppPassword
};

const adapter = new teams.TeamsAdapter(botSetting);

adapter.use(new teams.TeamsMiddleware());

// Catch-all for any unhandled errors in your bot.
adapter.onTurnError = async (turnContext, error) => {
    // This check writes out errors to console log .vs. app insights.
    console.error(`\n [onTurnError]: ${ error }`);
};

// Create the EchoBot.
const bot = new FileBot();

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open echobot-with-counter.bot file in the Emulator.`);
});

// Listen for incoming activities and route them to your bot for processing.
server.use(require('restify-plugins').bodyParser());
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        // Call bot.onTurn() to handle all incoming messages.
        await bot.onTurn(turnContext);
    });
});
