// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot

// Note: Ensure you have a .env file .
const path = require('path');
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Import required pckages

const restify = require('restify');

// Import required bot services. See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, MemoryStorage, ConversationState, UserState } = require('botbuilder');
const { ApplicationInsightsTelemetryClient, ApplicationInsightsWebserverMiddleware } = require('botbuilder-applicationinsights');

// This bot's main dialog.
const { DialogAndWelcomeBot } = require('./bots/dialogAndWelcomeBot');
const { MainDialog } = require('./dialogs/mainDialog');

const batchSender = require('./eventhubs/eventHubsBatchingSender');
const { TelemetryTranscriptLoggerMiddleware } = require('./eventhubs/telemetryTranscriptMiddleware');



// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Create an Application Insights BotTelemetryClient.
const appInsightsClient = new ApplicationInsightsTelemetryClient(process.env.AppInsightsInstrumentationKey);

// Register a TelemetryTranscriptLoggerMiddleware instance.
// This will send to Application Insights (with transcript property) for all incoming/outgoing Message-type activities.
adapter.use(new TelemetryTranscriptLoggerMiddleware(appInsightsClient, true));

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    await context.sendActivity(`Oops. Something went wrong!`);
    // Clear out state
    await conversationState.delete(context);
};

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state store to persist the dialog and user state between messages.
let conversationState, userState;

// For local development, in-memory storage is used.
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
conversationState = new ConversationState(memoryStorage);
userState = new UserState(memoryStorage);

// CAUTION: You must ensure your product environment has the NODE_ENV set
//          to use the Azure Blob storage or Azure Cosmos DB providers.

// Add botbuilder-azure when using any Azure services.
// const { BlobStorage } = require('botbuilder-azure');
// // Get service configuration
// const blobStorageConfig = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION_ID);
// const blobStorage = new BlobStorage({
//     containerName: (blobStorageConfig.container || DEFAULT_BOT_CONTAINER),
//     storageAccountOrConnectionString: blobStorageConfig.connectionString,
// });
// conversationState = new ConversationState(blobStorage);
// userState = new UserState(blobStorage);

// Pass in a logger to the bot. For this sample, the logger is the console, but alternatives such as Application Insights and Event Hub exist for storing the logs of the bot.
const logger = console;

appInsightsClient.client.addTelemetryProcessor(sendToEventHubs);

// Create the main dialog.
const dialog = new MainDialog(logger, appInsightsClient);
const bot = new DialogAndWelcomeBot(conversationState, userState, dialog, logger, appInsightsClient);

// Create HTTP server
let server = restify.createServer();

// Enable the Application Insights middleware, which helps correlate all activity
// based on the incoming request.
server.use(restify.plugins.bodyParser());
server.use(ApplicationInsightsWebserverMiddleware);

server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nSee https://aka.ms/connect-to-bot for more information`);
});

// Listen for incoming activities and route them to your bot main dialog.
server.post('/api/messages', (req, res) => {
    // Route received a request to adapter for processing
    adapter.processActivity(req, res, async (turnContext) => {
        // route to bot activity handler.
        await bot.run(turnContext);
    });
});

// Application Insights Telemetry Processor
// This pulls the transcripts and pushes them to Application Insights.
function sendToEventHubs(envelope, context) {
    const telemetryItem = envelope.data['baseData']; 

    // Contains a transcript?
    if ('properties' in telemetryItem && 'transcript' in telemetryItem.properties) {
        batchSender.getBatchSender().send(telemetryItem.properties['transcript']);

        // Cleanup in case you don't want this in App Insights.
        delete telemetryItem.properties['transcript'];
    }

    return true;
}


