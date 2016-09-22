var builder = require('botbuilder');
var restify = require('restify');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

var userStore = [];

// Every 5 seconds, check for new registered users and start a new dialog
setInterval(function () {
    var newAddresses = userStore.splice(0);
    newAddresses.forEach(function (address) {

        console.log('Starting survey for address:', address);

        // start survey dialog using stored address
        bot.beginDialog(address, '/survey');

    });
}, 5000);

bot.dialog('/', function (session) {
    var msg = session.message;

    // store user's address

    // minimal information copied from the address of a previous message
    // we should persist this object if we want to create a new conversation anytime later 
    var address = {
        channelId: msg.address.channelId,
        serviceUrl: msg.address.serviceUrl,
        user: msg.address.user,
        bot: msg.address.bot,
        useAuth: true
    };

    // store address
    userStore.push(address);

    // end current dialog
    session.endDialog('You\'ve been invited to a survey! It will start in a few seconds...');
});

bot.dialog('/survey', [
    function (session) {
        builder.Prompts.text(session, 'Hello... What\'s your name?');
    },
    function (session, results) {
        session.userData.name = results.response;
        builder.Prompts.number(session, 'Hi ' + results.response + ', How many years have you been coding?');
    },
    function (session, results) {
        session.userData.coding = results.response;
        builder.Prompts.choice(session, 'What language do you code Node using? ', ['JavaScript', 'CoffeeScript', 'TypeScript']);
    },
    function (session, results) {
        session.userData.language = results.response.entity;
        session.endDialog('Got it... ' + session.userData.name +
            ' you\'ve been programming for ' + session.userData.coding +
            ' years and use ' + session.userData.language + '.');
    }
]);