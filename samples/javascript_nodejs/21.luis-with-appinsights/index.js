// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const restify = require('restify');
const { BotFrameworkAdapter } = require('botbuilder');
<<<<<<< HEAD
=======
const { ApplicationInsightsTelemetryClient, ApplicationInsightsWebserverMiddleware } = require('botbuilder-applicationinsights');
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
const { BotConfiguration } = require('botframework-config');
const { LuisBot } = require('./bot');
const { MyAppInsightsMiddleware } = require('./middleware');

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, './.env');
<<<<<<< HEAD
const env = require('dotenv').config({ path: ENV_FILE });
=======
require('dotenv').config({ path: ENV_FILE });
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

// Get the .bot file path.
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));
let botConfig;
try {
<<<<<<< HEAD
    // Read bot configuration from .bot file. 
=======
    // Read bot configuration from .bot file.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.error(`\nError reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    console.error(`\n - The botFileSecret is available under appsettings for your Azure Bot Service bot.`);
    console.error(`\n - If you are running this bot locally, consider adding a .env file with botFilePath and botFileSecret.\n\n`);
    process.exit();
}

// For local development configuration as defined in .bot file.
const DEV_ENVIRONMENT = 'development';

// Bot name as defined in .bot file or from runtime.
// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = (process.env.NODE_ENV || DEV_ENVIRONMENT);

// LUIS and Application Insights service names as found in .bot file.
<<<<<<< HEAD
const LUIS_CONFIGURATION = 'reminders';
const APP_INSIGHTS_CONFIGURATION = 'appInsights';
=======
const LUIS_CONFIGURATION = 'luis-with-appinsights-luis';
const APP_INSIGHTS_CONFIGURATION = null; // Define a specific instance of Application Insights (if required)
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

// Get bot endpoint configuration by service name.
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
<<<<<<< HEAD
const appInsightsConfig = botConfig.findServiceByNameOrId(APP_INSIGHTS_CONFIGURATION);

=======
const appInsightsConfig = APP_INSIGHTS_CONFIGURATION
                                ?
                                botConfig.findServiceByNameOrId(APP_INSIGHTS_CONFIGURATION)
                                :
                                botConfig.services.filter((m) => m.type === 'appInsights').length ? botConfig.services.filter((m) => m.type === 'appInsights')[0] : null;
if (!appInsightsConfig) {
    throw new Error("No App Insights configuration was found.");
}
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
// Map the contents of luisConfig to a consumable format for our MyAppInsightsLuisRecognizer class.
const luisApplication = {
    applicationId: luisConfig.appId,
    endpointKey: luisConfig.subscriptionKey || luisConfig.authoringKey,
    endpoint: luisConfig.getEndpoint()
};

// Create two variables to indicate whether or not the bot should include messages' text and usernames when
// sending information to Application Insights.
// These settings are used in the MyApplicationInsightsMiddleware and also in MyAppInsightsLuisRecognizer.
const logMessage = true;
const logName = true;

// Map the contents of appInsightsConfig to a consumable format for MyAppInsightsMiddleware.
const appInsightsSettings = {
<<<<<<< HEAD
    instrumentationKey: appInsightsConfig.instrumentationKey,
=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    logOriginalMessage: logMessage,
    logUserName: logName
};

<<<<<<< HEAD
=======
// Create an Application Insights telemetry client.
const appInsightsClient = new ApplicationInsightsTelemetryClient(appInsightsConfig.instrumentationKey);

>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
// Indicate that the base LuisRecognizer class should include the raw LUIS results.
const includeApiResults = true;

// Create adapter. See https://aka.ms/about-bot-adapter to learn more adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword
});

// Register a MyAppInsightsMiddleware instance.
// This will send to Application Insights all incoming Message-type activities.
// It also stores in TurnContext.TurnState an `applicationinsights` TelemetryClient instance.
// This cached TelemetryClient is used by the custom class MyAppInsightsLuisRecognizer.
<<<<<<< HEAD
adapter.use(new MyAppInsightsMiddleware(appInsightsSettings));

// Catch-all for errors.
adapter.onTurnError = async (turnContext, error) => {
    console.error(`\n [onTurnError]: ${error}`);
    await turnContext.sendActivity(`Oops. Something went wrong!`);
=======
adapter.use(new MyAppInsightsMiddleware(appInsightsClient, appInsightsSettings));

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    console.error(`\n [onTurnError]: ${ error }`);
    await context.sendActivity(`Oops. Something went wrong!`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
};

// Create the LuisBot.
let bot;
try {
    bot = new LuisBot(luisApplication,
        {
            includeAllIntents: true,
            log: false,
            staging: false
        },
        includeApiResults,
        logMessage,
        logName);
} catch (err) {
<<<<<<< HEAD
    console.error(`[botInitializationError]: ${err}`);
=======
    console.error(`[botInitializationError]: ${ err }`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    process.exit();
}

// Create HTTP server.
let server = restify.createServer();
<<<<<<< HEAD
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}.`);
=======

// Enable the Application Insights middleware, which helps correlate all activity
// based on the incoming request.
server.use(ApplicationInsightsWebserverMiddleware);

server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }.`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator.`);
    console.log(`\nTo talk to your bot, open luis-with-appinsights.bot file in the emulator.`);
});

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (context) => {
        await bot.onTurn(context);
    });
});
