// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import * as restify from 'restify';
import { TeamsAdapter, TeamsMiddleware } from 'botbuilder-teams';
import { BotFrameworkAdapterSettings } from 'botbuilder';
import { FileBot } from './bot';
import * as path from 'path';
import { config } from 'dotenv';

// Note: Ensure you have a .env file and include MicrosoftAppId and MicrosoftAppPassword.
const ENV_FILE = path.join(__dirname, '..', '.env');
config({path: ENV_FILE});

const botSetting: Partial<BotFrameworkAdapterSettings> = {
  appId: process.env.MicrosoftAppId,
  appPassword: process.env.MicrosoftAppPassword
};

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new TeamsAdapter(botSetting);

adapter.use(new TeamsMiddleware());

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error('\n [onTurnError]:')
    console.error(error);
    // Send a message to the user
    await context.sendActivity(`Oops. Something went wrong!`);
};

const bot = new FileBot();

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
});

// Listen for incoming activities and route them to your bot for processing.
server.use(require('restify-plugins').bodyParser());
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        // Route to bot activity handler.
        await bot.run(turnContext);
    });
});
