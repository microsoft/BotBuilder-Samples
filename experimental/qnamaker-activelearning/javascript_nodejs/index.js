// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const restify = require('restify');
const { BotFrameworkAdapter, ConversationState, MemoryStorage } = require('botbuilder');
const { BotConfiguration } = require('botframework-config');
const { QnAMakerBot } = require('./bot');

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

console.log(process.env.botFilePath);

// Get the .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));
let botConfig;
try {
    // Read configuration from .bot file.
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(err);
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

// QnA Maker service name as specified in .bot file.
const QNA_CONFIGURATION = 'qnamakerService';

// Get endpoint and QnA Maker configurations by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const qnaConfig = botConfig.findServiceByNameOrId(QNA_CONFIGURATION);

// Map the contents to the required format for `QnAMaker`.
const qnaEndpointSettings = {
    knowledgeBaseId: qnaConfig.kbId,
    endpointKey: qnaConfig.endpointKey,
    host: qnaConfig.hostname
};

const memoryStorage = new MemoryStorage();
const conversationState = new ConversationState(memoryStorage);

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    console.error(`\n [onTurnError]: ${ error }`);
    await context.sendActivity(`Oops. Something went wrong!`);
};

// Create the QnAMakerBot.
let bot;
try {
    bot = new QnAMakerBot(qnaEndpointSettings, conversationState);
} catch (err) {
    console.error(`[botInitializationError]: ${ err }`);
    process.exit();
}

// Create HTTP server.
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 39781, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open qnamaker.bot file in the emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        await bot.onTurn(turnContext);
    });
});
