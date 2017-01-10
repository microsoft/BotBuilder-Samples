var builder = require('botbuilder');
var restify = require('restify');
require('dotenv-extended').load();
var telemetryModule = require('./telemetry-module.js');

var appInsights = require('applicationinsights');
appInsights.setup(process.env.APPINSIGHTS_INSTRUMENTATIONKEY).start();
var appInsightsClient = appInsights.getClient();

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

// Listen for messages
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

        telemetry.setDefault = true;
    }

    var defaultCity = session.conversationData[CityKey];
    session.send('Welcome to the Search City bot. I\'m currently configured to search for things in %s', defaultCity);

    appInsightsClient.trackTrace('start', telemetry);

    session.beginDialog('search');
});

// Enable Conversation Data persistence
bot.set('persistConversationData', true);

// Main dialog
bot.dialog('search', new builder.IntentDialog()
    .onBegin(function (session, args, next) {
        // is user's name set? 
        var userName = session.userData[UserNameKey];
        if (!userName) {
            session.beginDialog('greet');
            return;
        }

        try {
            // has the user been welcomed to the conversation?
            if (!session.privateConversationData[UserWelcomedKey]) {
                session.privateConversationData[UserWelcomedKey] = true;
                session.send('Welcome back %s! Remember the rules: %s', userName, HelpMessage);
            }
        } catch (error) {
            var exceptionTelemetry = telemetryModule.createTelemetry(session);
            exceptionTelemetry.exception = error.toString();
            appInsightsClient.trackException(exceptionTelemetry);

        } finally {
            var resumeAfterPromptTelemetry = telemetryModule.createTelemetry(session);
            appInsightsClient.trackTrace('resumeAfterPrompt', resumeAfterPromptTelemetry);
        }

        next();

    }).matches(/^current city/i, function (session) {
        // print city settings
        var userName = session.userData[UserNameKey];
        var defaultCity = session.conversationData[CityKey];
        var userCity = session.privateConversationData[CityKey];
        if (userCity) {
            session.send(
                '%s, you have overridden the city. Your searches are for things in %s. The default conversation city is %s.',
                userName, userCity, defaultCity);
            return;
        } else {
            session.send('Hey %s, I\'m currently configured to search for things in %s.', userName, defaultCity);
        }

        var telemetry = telemetryModule.createTelemetry(session);
        appInsightsClient.trackEvent('current city', telemetry);

    }).matches(/^change city to (.*)/i, function (session, args) {
        // change default city
        var newCity = args.matched[1].trim();
        session.conversationData[CityKey] = newCity;
        var userName = session.userData[UserNameKey];
        session.send('All set %s. From now on, all my searches will be for things in %s.', userName, newCity);

        var telemetry = telemetryModule.createTelemetry(session);
        appInsightsClient.trackEvent('change city to', telemetry);

    }).matches(/^change my city to (.*)/i, function (session, args) {
        // change user's city
        var newCity = args.matched[1].trim();
        session.privateConversationData[CityKey] = newCity;
        var userName = session.userData[UserNameKey];
        session.send('All set %s. I have overridden the city to %s just for you', userName, newCity);
        var telemetry = telemetryModule.createTelemetry(session);
        appInsightsClient.trackEvent('change my city to', telemetry);

    }).matches(/^reset/i, function (session, args) {
        // reset data
        delete session.userData[UserNameKey];
        delete session.privateConversationData[CityKey];
        delete session.privateConversationData[UserWelcomedKey];
        session.send('Ups... I\'m suffering from a memory loss...');
        session.endDialog();

    }).onDefault(function (session) {
        // perform search
        var measuredEventTelemetry = telemetryModule.createTelemetry(session);
        var timerStart = process.hrtime();

        try {
            var city = session.privateConversationData[CityKey] || session.conversationData[CityKey];
            var userName = session.userData[UserNameKey];
            var messageText = session.message.text.trim();
            session.send('%s, wait a few seconds. Searching for \'%s\' in \'%s\'...', userName, messageText, city);
            session.send('https://www.bing.comsearch?q=%s', encodeURIComponent(messageText + ' in ' + city));

        } catch (error) {
            measuredEventTelemetry.exception = error.toString();
            appInsightsClient.trackException('search', t);

        } finally {
            var timerEnd = process.hrtime(timerStart);
            measuredEventTelemetry.metrics = (timerEnd[0], timerEnd[1] / 1000000);
            appInsightsClient.trackEvent('timeTaken', measuredEventTelemetry);

        }
    }));

// Greet dialog
bot.dialog('greet', new builder.SimpleDialog(function (session, results) {
    if (results && results.response) {
        session.userData[UserNameKey] = results.response;
        session.privateConversationData[UserWelcomedKey] = true;
        session.send('Welcome %s! %s', results.response, HelpMessage);

        var telemetry = telemetryModule.createTelemetry(session);
        telemetry.userName = results.response; // You can add properties after-the-fact as well
        appInsightsClient.trackEvent('new user', telemetry);

        //  end the current dialog and replace it with  'search' dialog
        session.replaceDialog('search');
        return;
    }

    builder.Prompts.text(session, 'Before get started, please tell me your name?');
}));