// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const path = require('path');

const dotenv = require('dotenv');
// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const {
    CloudAdapter,
    ConfigurationBotFrameworkAuthentication
} = require('botbuilder');

const { AuthenticationConstants } = require('botframework-connector');

// This bot's main dialog.
const { NamedPipeBot } = require('./bot');

const generateDirectLineToken = require('./utils/generateDirectLineToken');
const renewDirectLineToken = require('./utils/renewDirectLineToken');

// Create HTTP server
const server = restify.createServer();
server.use(restify.plugins.bodyParser());

server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log('\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator');
    console.log('\nTo talk to your bot, open the emulator select "Open Bot"');
});

const botFrameworkAuthentication = new ConfigurationBotFrameworkAuthentication(process.env);

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about how bots work.
const adapter = new CloudAdapter(botFrameworkAuthentication);

adapter.connectNamedPipe(
    `${ process.env.WEBSITE_NAME }.directline`,
    async (context) => {
        await myBot.run(context);
    },
    process.env.MicrosoftAppId ?? '',
    AuthenticationConstants.ToChannelFromBotOAuthScope);

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
const myBot = new NamedPipeBot();

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

// Token Server, to ensure DL secret is not visible to client
server.post('/api/token/directlinease', async (req, res) => {
    const { DIRECT_LINE_SECRET, WEBSITE_NAME } = process.env;

    try {
        if (!WEBSITE_NAME) {
            return res.send(500, 'only available on azure', { 'Access-Control-Allow-Origin': '*' });
        }

        const { token } = req.query;

        try {
            const result = await (token
                ? renewDirectLineToken(token, { domain: `https://${ WEBSITE_NAME }.azurewebsites.net/.bot/` })
                : generateDirectLineToken(DIRECT_LINE_SECRET, { domain: `https://${ WEBSITE_NAME }.azurewebsites.net/.bot/` }));

            res.sendRaw(JSON.stringify(result, null, 2), {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json'
            });
        } catch (err) {
            res.send(500, err.message);
        }

        console.log('token= ' + token);

        if (token) {
            console.log('Refreshing Direct Line ASE token');
        } else {
            console.log(
                `Requesting Direct Line ASE token using secret "${ DIRECT_LINE_SECRET?.substring(
                    0,
                    3
                ) }...${ DIRECT_LINE_SECRET?.substring(-3) }"`
            );
        }
    } catch (err) {
        console.log('err= ' + err);
        res.send(500, { message: err.message, stack: err.stack }, { 'Access-Control-Allow-Origin': '*' });
    }
});
