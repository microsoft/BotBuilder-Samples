// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');

var telemetryModule = require('./telemetry-module.js');

var appInsights = require('applicationinsights');
appInsights.setup(process.env.APPINSIGHTS_INSTRUMENTATION_KEY).start();
var appInsightsClient = appInsights.getClient();

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create connector and listen for messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

server.post('/api/messages', connector.listen());

var HelpMessage = '\n * If you want to know which city I\'m using for my searches type \'current city\'. \n * Want to change the current city? Type \'change city to cityName\'. \n * Want to change it just for your searches? Type \'change my city to cityName\'';
var UserNameKey = 'UserName';
var UserWelcomedKey = 'UserWelcomed';
var CityKey = 'City';

// Setup bot with default dialog
var bot = new builder.UniversalBot(connector, function (session) {

    var telemetry = telemetryModule.createTelemetry(session, { setDefault: false });

    // initialize with default city
    if (!session.conversationData[CityKey]) {
        session.conversationData[CityKey] = 'Seattle';
        session.send('Welcome to the Search City bot. I\'m currently configured to search for things in %s', session.conversationData[CityKey]);
    }

    appInsightsClient.trackTrace('start', telemetry);

    // is user's name set? 
    var userName = session.userData[UserNameKey];
    if (!userName) {
        return session.beginDialog('greet');
    }

    try {
        // has the user been welcomed to the conversation?
        if (!session.privateConversationData[UserWelcomedKey]) {
            session.privateConversationData[UserWelcomedKey] = true;
            return session.send('Welcome back %s! Remember the rules: %s', userName, HelpMessage);
        }
    } catch (error) {
        var exceptionTelemetry = telemetryModule.createTelemetry(session);
        exceptionTelemetry.exception = error.toString();
        appInsightsClient.trackException(exceptionTelemetry);
    } finally {
        var resumeAfterPromptTelemetry = telemetryModule.createTelemetry(session);
        appInsightsClient.trackTrace('resumeAfterPrompt', resumeAfterPromptTelemetry);
    }

    session.beginDialog('search');
});

// Enable Conversation Data persistence
bot.set('persistConversationData', true);

// search dialog
bot.dialog('search', function (session, args, next) {
    var measuredEventTelemetry = telemetryModule.createTelemetry(session);
    var timerStart = process.hrtime();

    try {
        // perform search
        var city = session.privateConversationData[CityKey] || session.conversationData[CityKey];
        var userName = session.userData[UserNameKey];
        var messageText = session.message.text.trim();
        session.send('%s, wait a few seconds. Searching for \'%s\' in \'%s\'...', userName, messageText, city);
        session.send('https://www.bing.com/search?q=%s', encodeURIComponent(messageText + ' in ' + city));
    } catch (error) {
        measuredEventTelemetry.exception = error.toString();
        appInsightsClient.trackException('search', t);
    } finally {
        var timerEnd = process.hrtime(timerStart);
        measuredEventTelemetry.metrics = (timerEnd[0], timerEnd[1] / 1000000);
        appInsightsClient.trackEvent('timeTaken', measuredEventTelemetry);
    }
    
    session.endDialog();
});

// reset bot dialog
bot.dialog('reset', function (session) {
    // reset data
    delete session.userData[UserNameKey];
    delete session.conversationData[CityKey];
    delete session.privateConversationData[CityKey];
    delete session.privateConversationData[UserWelcomedKey];

    var telemetry = telemetryModule.createTelemetry(session);
    appInsightsClient.trackEvent('reset', telemetry);

    session.endDialog('Ups... I\'m suffering from a memory loss...');
}).triggerAction({ matches: /^reset/i });

// print current city dialog
bot.dialog('printCurrentCity', function (session) {
    // print city settings
    var userName = session.userData[UserNameKey];
    var defaultCity = session.conversationData[CityKey];
    var userCity = session.privateConversationData[CityKey];
    if (userCity) {
        session.endDialog(
            '%s, you have overridden the city. Your searches are for things in %s. The default conversation city is %s.',
            userName, userCity, defaultCity);
    } else {
        session.endDialog('Hey %s, I\'m currently configured to search for things in %s.', userName, defaultCity);
    }

    var telemetry = telemetryModule.createTelemetry(session);
    appInsightsClient.trackEvent('current city', telemetry);

    session.endDialog();
}).triggerAction({ matches: /^current city/i });

// change current city dialog
bot.dialog('changeCurrentCity', function (session, args) {
    // change default city
    var newCity = args.intent.matched[1].trim();
    session.conversationData[CityKey] = newCity;
    var userName = session.userData[UserNameKey];

    var telemetry = telemetryModule.createTelemetry(session);
    appInsightsClient.trackEvent('change city to', telemetry);

    session.endDialog('All set %s. From now on, all my searches will be for things in %s.', userName, newCity);
}).triggerAction({ matches: /^change city to (.*)/i });

// change my current city dialog
bot.dialog('changeMyCurrentCity', function (session, args) {
    // change user's city
    var newCity = args.intent.matched[1].trim();
    session.privateConversationData[CityKey] = newCity;
    var userName = session.userData[UserNameKey];

    var telemetry = telemetryModule.createTelemetry(session);
    appInsightsClient.trackEvent('change my city to', telemetry);

    session.endDialog('All set %s. I have overridden the city to %s just for you', userName, newCity);
}).triggerAction({ matches: /^change my city to (.*)/i });

// Greet dialog
bot.dialog('greet', new builder.SimpleDialog(function (session, results) {
    if (results && results.response) {
        session.userData[UserNameKey] = results.response;
        session.privateConversationData[UserWelcomedKey] = true;

        var telemetry = telemetryModule.createTelemetry(session);
        telemetry.userName = results.response; // You can add properties after-the-fact as well
        appInsightsClient.trackEvent('new user', telemetry);

        return session.endDialog('Welcome %s! %s', results.response, HelpMessage);
    }

    builder.Prompts.text(session, 'Before get started, please tell me your name?');
}));
