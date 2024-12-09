// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const dotenv = require('dotenv');
const restify = require('restify');
const { FederatedServiceClientCredentialsFactory } = require('botframework-connector');

// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const {
    CloudAdapter,
    ConfigurationBotFrameworkAuthentication
} = require('botbuilder');

// This bot's main dialog.
const { EchoBot } = require('./bot');

(async () => {
    try {
        // Create HTTP server.
        const server = restify.createServer();
        server.use(restify.plugins.bodyParser());

        server.listen(process.env.port || process.env.PORT || 3978, () => {
            console.log(`\n${ server.name } listening to ${ server.url }`);
            console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
            console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
        });

        // Create the Federated Service Client Credentials to be used as the ServiceClientCredentials for the Bot Framework SDK.
        const serviceClientCredentialsFactory = new FederatedServiceClientCredentialsFactory(
            process.env.MicrosoftAppId,
            process.env.MicrosoftAppClientId,
            process.env.MicrosoftAppTenantId
        );

        const botFrameworkAuthentication = new ConfigurationBotFrameworkAuthentication(process.env, serviceClientCredentialsFactory);

        // Create adapter.
        // See https://aka.ms/about-bot-adapter to learn more about how bots work.
        const adapter = new CloudAdapter(botFrameworkAuthentication);
        // Catch-all for errors.
        const onTurnErrorHandler = async (context, error) => {
            // This check writes out errors to console log .vs. app insights.
            // NOTE: In production environment, you should consider logging this to Azure
            //       application insights. See https://aka.ms/bottelemetry for telemetry
            //       configuration instructions.
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
        };

        // Set the onTurnError for the singleton CloudAdapter.
        adapter.onTurnError = onTurnErrorHandler;

        // Create the main dialog.
        const myBot = new EchoBot();

        // Listen for incoming requests.
        server.post('/api/messages', async (req, res) => {
            // Route received a request to adapter for processing
            await adapter.process(req, res, (context) => myBot.run(context));
        });

        // Listen for Upgrade requests for Streaming.
        server.on('upgrade', async (req, socket, head) => {
            // Create an adapter scoped to this WebSocket connection to allow storing session data.
            const streamingAdapter = new CloudAdapter(botFrameworkAuthentication);
            // Set onTurnError for the CloudAdapter created for each connection.
            streamingAdapter.onTurnError = onTurnErrorHandler;

            await streamingAdapter.process(req, socket, head, (context) => myBot.run(context));
        });
    } catch (error) {
        console.log(error);
    }
})();
