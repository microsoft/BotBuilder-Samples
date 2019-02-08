// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
import {  ActivityTypes, BotFrameworkAdapter, ConversationState, MemoryStorage } from 'botbuilder';
import { DialogSet, DialogTurnStatus, TextPrompt,  WaterfallDialog } from 'botbuilder-dialogs';

// Import the ActivityHandler class onto which handlers will be bound.
import { ActivityHandler } from './ActivityHandler';

// Import required bot configuration.
import { BotConfiguration, IEndpointService } from 'botframework-config';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '..', '.env');
config({ path: ENV_FILE });

// bot endpoint name as defined in .bot file
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
const DEV_ENVIRONMENT = 'development';

// bot name as defined in .bot file
// See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
const BOT_CONFIGURATION = (process.env.NODE_ENV || DEV_ENVIRONMENT);

// Create HTTP server.
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open sample.bot file in the Emulator.`);
});

// .bot file path
const BOT_FILE = path.join(__dirname, '..', (process.env.botFilePath || ''));

// Read bot configuration from .bot file.
let botConfig = {};
try {
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.error(`\nError reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    console.error(`\n - The botFileSecret is available under appsettings for your Azure Bot Service bot.`);
    console.error(`\n - If you are running this bot locally, consider adding a .env file with botFilePath and botFileSecret.\n\n`);
    process.exit();
}

// Get bot endpoint configuration by service name
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION) as IEndpointService;

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about .bot file its use and bot configuration.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword,
});

const storage = new MemoryStorage({});

const conversationState = new ConversationState(storage);
const dialogState = conversationState.createProperty('dialogState');
const dialogSet = new DialogSet(dialogState);

dialogSet.add(new TextPrompt('textPrompt'));

dialogSet.add(new WaterfallDialog('hello', [
    async (step) => {
        await step.context.sendActivity('Hi!');
        return step.next();
    },
    async (step) => {
        return step.prompt('textPrompt', 'How are you?');
    },
    async (step) => {
        await step.context.sendActivity('ACKNOWLEDGED.');
        return step.next();
    },
]));

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log .vs. app insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    await context.sendActivity(`Oops. Something went wrong!`);
};

// Create the main dialog.
const myBot = new ActivityHandler();

myBot.onTurn(async (context, next) => {
    // await context.sendActivity(`Received activity of type ${ context.activity.type }`);
    await next();
});

myBot.onMessage(async (context, next) => {
    // await context.sendActivity(`Echo: ${ context.activity.text }`);
    await next();
});

myBot.onMessage(async (context, next) => {
    // await context.sendActivity(`Echo: ${ context.activity.text }`);

    if (context.activity.text.match(/help/i)) {
        await context.sendActivity(`YOU NEED HELP! I saw this in my onMessage handler and will stop processing this message any further.`);
    } else {
        await next();
    }

});

myBot.onConversationUpdate(async (context, next) => {
    await context.sendActivity('Something about this conversation has been updated.');
    await next();
});

myBot.onMembersAdded(async (context, next) => {
    await context.sendActivity('Members have been added.');
    await next();
});

myBot.onMembersRemoved(async (context, next) => {
    await context.sendActivity('Members have been removed.');
    await next();
});

myBot.onUnrecognizedActivityType(async (context, next) => {
    await context.sendActivity(`I am not sure to do with an event of type ${ context.activity.type }.`);
    await next();
});

myBot.onInvoke(async (context, next) => {

    await next();

    // return an invoke response which is sent directly via the http response
    return {
        status: 200,
        body: 'invoked',
    };
});


myBot.onDialog(async (context, next) => {

    if (context.activity.type === ActivityTypes.Message) {
        const dialogContext = await dialogSet.createContext(context);
        const results = await dialogContext.continueDialog();

        // fallback behavior is to run welcome dialog
        if (results.status === DialogTurnStatus.empty) {
        await dialogContext.beginDialog('hello');
        }

        await conversationState.saveChanges(context);
    }

    await next();

});

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // Route to main dialog.
        await myBot.run(context);
    });
});
