// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const dotenv = require('dotenv');
const path = require('path');
const restify = require('restify');
const { name } = require('./package.json');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, InspectionMiddleware, MemoryStorage, InspectionState, UserState, ConversationState } = require('botbuilder');
const { ActivityTypes } = require('botbuilder-core');

const { MicrosoftAppCredentials } = require('botframework-connector');

// This bot's main dialog.
const { IntersectionBot } = require('./bot');

// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

// Create HTTP server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open the emulator select "Open Bot"`);
});

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about how bots work.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Create the Storage provider and the various types of BotState.
const memoryStorage = new MemoryStorage();
const inspectionState = new InspectionState(memoryStorage);
const userState = new UserState(memoryStorage);
const conversationState = new ConversationState(memoryStorage);

// Create and add the InspectionMiddleware to the adapter.
adapter.use(new InspectionMiddleware(inspectionState, userState, conversationState, new MicrosoftAppCredentials(process.env.MicrosoftAppId, process.env.MicrosoftAppPassword)));

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // Create a trace activity that contains the error object
    const traceActivity = {
        type: ActivityTypes.Trace,
        timestamp: new Date(),
        name: 'Turn Error',
        label: 'TurnError',
        value: `${ error }`,
        valueType: 'https://www.botframework.com/schemas/error'
    };
    // This check writes out errors to console log .vs. app insights.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);

    // Send a trace activity, which will be displayed in Bot Framework Emulator
    await context.sendActivity(traceActivity);

    // Send a message to the user
    await context.sendActivity(`Bot Framework encounted an error or bug in ${ name }.`);
    await context.sendActivity(`To continue to run this bot, please fix ${ name } source code.`);
    // Clear out state
    await conversationState.clear(context);
};

// Create the main dialog.
const bot = new IntersectionBot(conversationState, userState);

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // Route to main dialog.
        await bot.run(context);
    });
});
