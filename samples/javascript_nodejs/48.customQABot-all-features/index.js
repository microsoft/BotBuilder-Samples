// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @ts-check

// index.js is used to setup and configure your bot

// Import required packages
const path = require('path');

// Note: Ensure you have a .env file and include ProjectName, LanguageEndpointKey and LanguageEndpointHostName.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, ConversationState, MemoryStorage, UserState } = require('botbuilder');

const { CustomQABot } = require('./bots/CustomQABot');
const { RootDialog } = require('./dialogs/rootDialog');

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log .vs. app insights.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights. See https://aka.ms/bottelemetry for telemetry
    //       configuration instructions.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);
    console.error(error);

    // Send a trace activity, which will be displayed in Bot Framework Emulator
    await context.sendTraceActivity(
        'OnTurnError Trace',
        `${ error }`,
        'https://www.botframework.com/schemas/error',
        'TurnError'
    );

    // Send a message to the user
    await context.sendActivity('The bot encountered an error or bug.');
    await context.sendActivity('To continue to run this bot, please fix the bot source code.');
};

// Define the state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state storage system to persist the dialog and user state between messages.

// For local development, in-memory storage is used.
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
const conversationState = new ConversationState(memoryStorage);
const userState = new UserState(memoryStorage);

var endpointHostName = process.env.LanguageEndpointHostName;
if (!endpointHostName?.startsWith('https://')) {
    endpointHostName = 'https://' + endpointHostName;
}

// To support backward compatibility for Key Names, fallback to process.env.QnAAuthKey.
const endpointKey = process.env.LanguageEndpointKey || process.env.QnAAuthKey;

// Create the main dialog.
const dialog = new RootDialog(
    process.env.ProjectName ?? '',
    endpointKey ?? '',
    endpointHostName,
    process.env.DefaultAnswer ?? '',
    process.env.EnablePreciseAnswer?.toLowerCase() ?? 'false',
    process.env.DisplayPreciseAnswerOnly?.toLowerCase() ?? 'false',
    process.env.UseTeamsAdaptiveCard?.toLowerCase());

// Create the bot's main handler.
const bot = new CustomQABot(conversationState, userState, dialog);

// Create HTTP server.
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    adapter.processActivity(req, res, async (turnContext) => {
        // Route the message to the bot's main handler.
        await bot.run(turnContext);
    });
});
