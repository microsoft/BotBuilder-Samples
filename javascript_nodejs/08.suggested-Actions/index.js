// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const restify = require('restify');
const SuggestedActionsBot = require('./bot');

const CONFIG_ERROR = 1;

// Import reuqired bot services. See https://ama.ms/bot-services to learn more about the different part of a bot
const { BotFrameworkAdapter } = require('botbuilder');

// Import required bot confuguration.
const { BotConfiguration } = require('botframework-config');

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
const env = require('dotenv').config({ path: ENV_FILE });

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${server.name} listening to ${server.url}.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open suggested-actions.bot file in the Emulator.`);
});

// .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

// Read bot configuration from .bot file. 
let botConfig;
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Bot name as defined in .bot file. 
// See https://aka.ms/about-bot-file to learn more about .bot file usage and configuration.
const BOT_CONFIGURATION = 'suggested-actions';

// Get bot endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// Create the bot that will handle incoming messages.
const suggestedActionsBot = new SuggestedActionsBot();

// Listen for incoming requests. 
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async(context) => {
        await suggestedActionsBot.onTurn(context);
    });
});