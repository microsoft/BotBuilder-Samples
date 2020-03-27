// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot

// Import required pckages
const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, ConversationState, MemoryStorage, UserState } = require('botbuilder');

const { AuthBot } = require('./bots/authBot');
const { MainDialog } = require('./dialogs/mainDialog');
const { OAuthAdapter } = require('./oauth_adapter');

// Read botFilePath and botFileSecret from .env file.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
// const adapter = new BotFrameworkAdapter({
//     appId: process.env.MicrosoftAppId,
//     appPassword: process.env.MicrosoftAppPassword
// });

const adapter = new OAuthAdapter(
{
    onTurnError: async (context, error) => {
        // This check writes out errors to console log .vs. app insights.
        // NOTE: In production environment, you should consider logging this to Azure
        //       application insights.
        console.error(`\n [onTurnError] unhandled error: ${ error }`);
    
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
        // Clear out state
        await conversationState.delete(context);
    },
    appId: process.env.MicrosoftAppId,
    password: process.env.MicrosoftAppPassword,
    fb_verify_token: process.env.fb_verify_token,
    fb_password: process.env.fb_password,
    fb_access_token: process.env.fb_access_token

}
);


// Define the state store for your bot.
// See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state storage system to persist the dialog and user state between messages.
const memoryStorage = new MemoryStorage();

// Create conversation and user state with in-memory storage provider.
const conversationState = new ConversationState(memoryStorage);
const userState = new UserState(memoryStorage);

// Create the main dialog.
const dialog = new MainDialog();
// Create the bot that will handle incoming messages.
const bot = new AuthBot(conversationState, userState, dialog);

// Create HTTP server.
const server = restify.createServer();
server.use(restify.plugins.bodyParser());
server.use(restify.plugins.queryParser());

server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        await bot.run(context);
    });
});

server.get('/api/messages', (req, res) => {
    //console.log(req.query['hub.mode'])
    //console.log(req)
     if (req.query['hub.mode'] === 'subscribe') {
          if (req.query['hub.verify_token'] === FB_VERIFY_TOKEN) {
               const val = req.query['hub.challenge'];
               res.sendRaw(200, val);
          } else {
               console.log('failed to verify endpoint');
               res.send('OK');
          }
     }
});
