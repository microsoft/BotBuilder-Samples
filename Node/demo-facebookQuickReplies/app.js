// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');

var locationPrompt = require('./prompts/location-prompt');

// Utility for simplifying construction of quick replies.
var quickReplies = require('./facebook/quickreplies');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot and listen to messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
server.post('/api/messages', connector.listen());

var bot = new builder.UniversalBot(connector, [
    function (session, args, next) {
        session.beginDialog('/startdemo');
    },
    function (session, args, next) {

    }
]);

// Create custom location prompt
locationPrompt.create(bot);

// This middleware is executed when any new messages are received by the bot.
// We check to see if the channel source is facebook and wether the message was
// triggered by a quick reply, the message text is replaced with the value of the
// quick reply so that dialog's or recognisers can process the message text as
// normal.
bot.use({
    botbuilder: function (session, next) {
        if (session.message.source == "facebook") {
            if (session.message.sourceEvent && session.message.sourceEvent.message) {
                if (session.message.sourceEvent.message.quick_reply) {
                    session.message.text = session.message.sourceEvent.message.quick_reply.payload;
                    session.send(`Quick reply text received: ${session.message.text}`);
                    session.endDialog();
                }
            }
        }

        next();
    }
});

// Starts the demo and provides the user with a choice of demos.
bot.dialog('/startdemo', [
    function (session, args, next) {
        builder.Prompts.choice(session, 'This bot demonstrates using Facebook Quick Replies, which demo would you like to see?', ['Text', 'Location']);
    },
    function (session, args, next) {
        if (args.response) {
            var choice = args.response.entity;
            switch (choice) {
                case 'Text':
                    session.replaceDialog('/textdemo');
                    break;
                case 'Location':
                    session.replaceDialog('/locationdemo');
            }
        }
    }
]);

// Demonstrates text based quick replies
bot.dialog('/textdemo', [
    function (session, args, next) {
        var message = new builder.Message(session).text('Pick a size:');
        quickReplies.AddQuickReplies(session, message, [
            new quickReplies.QuickReplyText('Small', 'small'),
            new quickReplies.QuickReplyText('Medium', 'medium'),
            new quickReplies.QuickReplyText('Large', 'large')
        ]);

        session.send(message);
    }
]);

// Demonstrates location based quick replies
bot.dialog('/locationdemo', [
    function (session, args, next) {
        locationPrompt.beginDialog(session);
    },
    function (session, args, next) {
        if (args.response) {
            var location = args.response.entity;
            session.send(`Location received: ${location.title}, lat: ${location.coordinates.lat}, long: ${location.coordinates.long}`);
            session.endDialog();
        } else {
            session.send('No location received');
            session.endDialog();            
        }
    }
])