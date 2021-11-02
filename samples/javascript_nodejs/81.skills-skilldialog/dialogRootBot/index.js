// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot.

// Import required packages.
const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const {
    ActivityTypes,
    ChannelServiceRoutes,
    CloudAdapter,
    CloudSkillHandler,
    ConfigurationServiceClientCredentialFactory,
    ConversationState,
    createBotFrameworkAuthenticationFromConfiguration,
    InputHints,
    MemoryStorage,
    SkillConversationIdFactory,
    TurnContext
} = require('botbuilder');
const {
    allowedCallersClaimsValidator,
    AuthenticationConfiguration,
    AuthenticationConstants
} = require('botframework-connector');

// Note: Ensure you have a .env file and include LuisAppId, LuisAPIKey and LuisAPIHostName.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// This bot's main dialog.
const { RootBot } = require('./bots/rootBot');
const { MainDialog } = require('./dialogs/mainDialog');

// Import Skills modules.
const { SkillsConfiguration } = require('./skillsConfiguration');

// Load skills configuration.
const skillsConfig = new SkillsConfiguration();

const allowedSkills = Object.values(skillsConfig.skills).map(skill => skill.appId);

const claimsValidators = allowedCallersClaimsValidator(allowedSkills);

// If the MicrosoftAppTenantId is specified in the environment config, add the tenant as a valid JWT token issuer for Bot to Skill conversation.
// The token issuer for MSI and single tenant scenarios will be the tenant where the bot is registered.
let validTokenIssuers = [];
const { MicrosoftAppTenantId } = process.env;

if (MicrosoftAppTenantId) {
    // For SingleTenant/MSI auth, the JWT tokens will be issued from the bot's home tenant.
    // Therefore, these issuers need to be added to the list of valid token issuers for authenticating activity requests.
    validTokenIssuers = [
        `${ AuthenticationConstants.ValidTokenIssuerUrlTemplateV1 }${ MicrosoftAppTenantId }/`,
        `${ AuthenticationConstants.ValidTokenIssuerUrlTemplateV2 }${ MicrosoftAppTenantId }/v2.0/`,
        `${ AuthenticationConstants.ValidGovernmentTokenIssuerUrlTemplateV1 }${ MicrosoftAppTenantId }/`,
        `${ AuthenticationConstants.ValidGovernmentTokenIssuerUrlTemplateV2 }${ MicrosoftAppTenantId }/v2.0/`
    ];
}

// Define our authentication configuration.
const authConfig = new AuthenticationConfiguration([], claimsValidators, validTokenIssuers);

const credentialsFactory = new ConfigurationServiceClientCredentialFactory({
    MicrosoftAppId: process.env.MicrosoftAppId,
    MicrosoftAppPassword: process.env.MicrosoftAppPassword,
    MicrosoftAppType: process.env.MicrosoftAppType,
    MicrosoftAppTenantId: process.env.MicrosoftAppTenantId
});

const botFrameworkAuthentication = createBotFrameworkAuthenticationFromConfiguration(null, credentialsFactory, authConfig);

// Create adapter, passing in authConfig so that we can use skills.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new CloudAdapter(botFrameworkAuthentication);

// Use the logger middleware to log messages. The default logger argument for LoggerMiddleware is Node's console.log().
const { LoggerMiddleware } = require('./middleware/loggerMiddleware');
adapter.use(new LoggerMiddleware());

// Catch-all for errors.
const onTurnErrorHandler = async (context, error) => {
    // This check writes out errors to the console log, instead of to app insights.
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights. See https://aka.ms/bottelemetry for telemetry
    //       configuration instructions.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);

    await sendErrorMessage(context, error);
    await endSkillConversation(context);
    await clearConversationState(context);
};

async function sendErrorMessage(context, error) {
    try {
        // Send a message to the user.
        let onTurnErrorMessage = 'The bot encountered an error or bug.';
        await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage, InputHints.IgnoringInput);

        onTurnErrorMessage = 'To continue to run this bot, please fix the bot source code.';
        await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage, InputHints.ExpectingInput);

        // Send a trace activity, which will be displayed in Bot Framework Emulator.
        await context.sendTraceActivity(
            'OnTurnError Trace',
            `${ error }`,
            'https://www.botframework.com/schemas/error',
            'TurnError'
        );
    } catch (err) {
        console.error(`\n [onTurnError] Exception caught in sendErrorMessage: ${ err }`);
    }
}

async function endSkillConversation(context) {
    try {
        // Inform the active skill that the conversation is ended so that it has a chance to clean up.
        // Note: the root bot manages the ActiveSkillPropertyName, which has a value while the root bot
        // has an active conversation with a skill.
        const activeSkill = await conversationState.createProperty(mainDialog.ActiveSkillPropertyName).get(context);
        if (activeSkill) {
            const botId = process.env.MicrosoftAppId;

            let endOfConversation = {
                type: ActivityTypes.EndOfConversation,
                code: 'RootSkillError'
            };
            endOfConversation = TurnContext.applyConversationReference(
                endOfConversation, TurnContext.getConversationReference(context.activity), true);

            await conversationState.saveChanges(context, true);
            await skillClient.postActivity(botId, activeSkill.appId, activeSkill.skillEndpoint, skillsConfig.skillHostEndpoint, endOfConversation.conversation.id, endOfConversation);
        }
    } catch (err) {
        console.error(`\n [onTurnError] Exception caught on attempting to send EndOfConversation : ${ err }`);
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

// Set the onTurnError for the singleton CloudAdapter.
adapter.onTurnError = onTurnErrorHandler;

// Define a state store for your bot. See https://aka.ms/about-bot-state to learn more about using MemoryStorage.
// A bot requires a state store to persist the dialog and user state between messages.

// For local development, in-memory storage is used.
// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
const memoryStorage = new MemoryStorage();
const conversationState = new ConversationState(memoryStorage);

// Create the conversationIdFactory.
const conversationIdFactory = new SkillConversationIdFactory(new MemoryStorage());

// Create the skill client.
const skillClient = botFrameworkAuthentication.createBotFrameworkClient();

// Create the main dialog.
const mainDialog = new MainDialog(conversationState, skillsConfig, skillClient, conversationIdFactory);
const bot = new RootBot(conversationState, mainDialog);

// Create HTTP server.
// maxParamLength defaults to 100, which is too short for the conversationId created in skillConversationIdFactory.
// See: https://github.com/microsoft/BotBuilder-Samples/issues/2194.
const server = restify.createServer({ maxParamLength: 1000 });
server.use(restify.plugins.bodyParser());

server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

// Listen for incoming activities and route them to your bot main dialog.
server.post('/api/messages', async (req, res) => {
    // Route received a request to adapter for processing
    await adapter.process(req, res, (context) => bot.run(context));
});

// Create and initialize the skill classes.
const handler = new CloudSkillHandler(adapter, (context) => bot.run(context), conversationIdFactory, botFrameworkAuthentication);
const skillEndpoint = new ChannelServiceRoutes(handler);
skillEndpoint.register(server, '/api/skills');

// Listen for Upgrade requests for Streaming.
server.on('upgrade', async (req, socket, head) => {
    // Create an adapter scoped to this WebSocket connection to allow storing session data.
    const streamingAdapter = new CloudAdapter(botFrameworkAuthentication);

    // Set onTurnError for the CloudAdapter created for each connection.
    streamingAdapter.onTurnError = onTurnErrorHandler;

    await streamingAdapter.process(req, socket, head, (context) => bot.run(context));
});
