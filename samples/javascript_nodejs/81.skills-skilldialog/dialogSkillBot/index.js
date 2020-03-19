// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot.

// Import required packages.
const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { ActivityTypes, BotFrameworkAdapter, ConversationState, InputHints, MemoryStorage } = require('botbuilder');
const { AuthenticationConfiguration } = require('botframework-connector');

// This bot's main dialog.
const { SkillBot } = require('./bots/skillBot');
const { ActivityRouterDialog } = require('./dialogs/activityRouterDialog');
const { FlightBookingRecognizer } = require('./dialogs/flightBookingRecognizer');

// Note: Ensure you have a .env file and include LuisAppId, LuisAPIKey and LuisAPIHostName.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Import Skills modules.
const { allowedSkillsClaimsValidator: allowedCallersClaimsValidator } = require('./authentication/allowedCallersClaimsValidator');

// Define our authentication configuration.
const authConfig = new AuthenticationConfiguration([], allowedCallersClaimsValidator);

// Create adapter, passing in authConfig so that we can use skills.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword,
    authConfig: authConfig
});

// Catch-all for errors.
const onTurnErrorHandler = async (context, error) => {
    // This check writes out errors to the console log, instead of to app insights.
    // NOTE: In a production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);

    await sendErrorMessage(context, error);
    await sendEoCToParent(context, error);
    await clearConversationState(context);
}

async function sendErrorMessage(context, error) {
    try {
        // Send a trace activity, which will be displayed in Bot Framework Emulator.
        await context.sendTraceActivity(
            'OnTurnError Trace',
            `${ error }`,
            'https://www.botframework.com/schemas/error',
            'TurnError'
        );
    
        // Send a message to the user.
        let onTurnErrorMessage = 'The skill encountered an error or bug.';
        await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage, InputHints.ExpectingInput);
    
        onTurnErrorMessage = 'To continue to run this bot, please fix the bot source code.';
        await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage, InputHints.ExpectingInput);
    
        // Send a trace activity, which will be displayed in the Bot Framework Emulator.
        // Note: we return the entire exception in the value property to help the developer;
        // this should not be done in production.
        await context.sendTraceActivity('OnTurnError Trace', error.toString(), 'https://www.botframework.com/schemas/error', 'TurnError');
    } catch (err) {
        console.error(`\n [onTurnError] Exception caught in sendErrorMessage: ${ err }`);
    }
}

async function sendEoCToParent(context, error) {
    try {
        // Send an EndOfConversation activity to the skill caller with the error to end the conversation,
        // and let the caller decide what to do.
        const endOfConversation = {
            type: ActivityTypes.EndOfConversation,
            code: 'SkillError',
            text: error.toString()
        };
        await context.sendActivity(endOfConversation);
    } catch (err) {
        console.error(`\n [onTurnError] Exception caught in sendEoCToParent: ${ err }`);
    }
}

async function clearConversationState(context) {
    try {
        // Delete the conversationState for the current conversation to prevent the
        // bot from getting stuck in a error-loop caused by being in a bad state.
        // ConversationState should be thought of as similar to "cookie-state" for a Web page.
        await conversationState.delete(context);
    } catch (err) {
        console.error(`\n [onTurnError] Exception caught on attempting to Delete ConversationState : ${ err }`);
    }
}

// Set the onTurnError for the singleton BotFrameworkAdapter.
adapter.onTurnError = onTurnErrorHandler;

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state store to persist the dialog and user state between messages.

// For local development, in-memory storage is used.
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
const conversationState = new ConversationState(memoryStorage);

// Initialize LUIS Recognizer.
const { LuisAppId, LuisAPIKey, LuisAPIHostName } = process.env;
const luisConfig = { applicationId: LuisAppId, endpointKey: LuisAPIKey, endpoint: `https://${ LuisAPIHostName }` };

const luisRecognizer = new FlightBookingRecognizer(luisConfig);

// Create the activity router dialog.
const activityRouterDialog = new ActivityRouterDialog(conversationState, luisRecognizer);
const bot = new SkillBot(conversationState, activityRouterDialog);

// Create HTTP server.
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3979, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

// Expose the manifest.
server.get('/manifest/*', restify.plugins.serveStatic({ directory: './manifest', appendRequestPath: false }));

// Listen for incoming activities and route them to your bot main dialog.
server.post('/api/messages', (req, res) => {
    // Route received requests to the adapter for processing
    adapter.processActivity(req, res, async (turnContext) => {
        // Route requests to bot activity handler.
        await bot.run(turnContext);
    });
});

// Listen for Upgrade requests for Streaming.
server.on('upgrade', (req, socket, head) => {
    // Create an adapter scoped to this WebSocket connection to allow storing session data.
    const streamingAdapter = new BotFrameworkAdapter({
        appId: process.env.MicrosoftAppId,
        appPassword: process.env.MicrosoftAppPassword
    });
    // Set onTurnError for the BotFrameworkAdapter created for each connection.
    streamingAdapter.onTurnError = onTurnErrorHandler;

    streamingAdapter.useWebSocket(req, socket, head, async (context) => {
        // After connecting via WebSocket, run this logic for every request sent over
        // the WebSocket connection.
        await bot.run(context);
    });
});
