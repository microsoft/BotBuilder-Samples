// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const dotenv = require('dotenv');
const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const {
    ActivityTypes,
    CloudAdapter,
    ConfigurationServiceClientCredentialFactory,
    InputHints,
    ConfigurationBotFrameworkAuthentication
} = require('botbuilder');
const {
    allowedCallersClaimsValidator,
    AuthenticationConfiguration,
    AuthenticationConstants
} = require('botframework-connector');

// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

// This bot's main dialog.
const { EchoBot } = require('./bot');

// Create HTTP server
const server = restify.createServer();
server.use(restify.plugins.bodyParser());

server.listen(process.env.port || process.env.PORT || 39783, () => {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

// Expose the manifest
server.get('/manifest/*', restify.plugins.serveStatic({ directory: './manifest', appendRequestPath: false }));

const allowedCallers = (process.env.AllowedCallers || '').split(',').filter((val) => val) || [];

const claimsValidators = allowedCallersClaimsValidator(allowedCallers);

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

const botFrameworkAuthentication = new ConfigurationBotFrameworkAuthentication(process.env, credentialsFactory, authConfig);

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about how bots work.
const adapter = new CloudAdapter(botFrameworkAuthentication);

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to the console log, instead of to app insights.
    // NOTE: In a production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError] unhandled error: ${ error }`);

    await sendErrorMessage(context, error);
    await sendEoCToParent(context, error);
};

async function sendErrorMessage(context, error) {
    try {
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

// Create the main dialog.
const myBot = new EchoBot();

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    // Route received a request to adapter for processing
    await adapter.process(req, res, (context) => myBot.run(context));
});
