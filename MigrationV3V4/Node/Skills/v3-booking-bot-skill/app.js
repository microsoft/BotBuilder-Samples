// This loads the environment variables from the .env file
require('dotenv').config();

var builder = require('botbuilder');
var restify = require('restify');
const skills = require('botbuilder/skills-validator');
const { allowedCallersClaimsValidator } = require('./allowedCallersClaimsValidator');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3980, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Expose the manifest
server.get('/manifest/*', restify.plugins.serveStatic({ directory: './manifest', appendRequestPath: false }));

// Create chat bot and listen to messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD,
    enableSkills: true,
    authConfiguration: new skills.AuthenticationConfiguration([], allowedCallersClaimsValidator)
});
server.post('/api/messages', connector.listen());

var DialogLabels = {
    Hotels: 'Hotels',
    Flights: 'Flights',
    Support: 'Support'
};

// Bot Storage: Here we register the state storage for your bot. 
// Default store: volatile in-memory store - Only for prototyping!
// We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
// For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
var inMemoryStorage = new builder.MemoryBotStorage();

var bot = new builder.UniversalBot(connector, [
    function (session) {
        // prompt for search option
        builder.Prompts.choice(
            session,
            'Are you looking for a flight or a hotel?',
            [DialogLabels.Flights, DialogLabels.Hotels],
            {
                maxRetries: 3,
                retryPrompt: 'Not a valid option'
            });
    },
    function (session, result) {
        if (!result.response) {
            // exhausted attemps and no selection, start over
            session.send('Ooops! Too many attemps :( But don\'t worry, I\'m handling that exception and you can try again!');
            return session.endDialog();
        }

        // on error, start over
        session.on('error', function (err) {
            session.send('Failed with message: %s', err.message);
            session.endDialog();
        });

        // continue on proper dialog
        var selection = result.response.entity;
        switch (selection) {
            case DialogLabels.Flights:
                return session.beginDialog('flights');
            case DialogLabels.Hotels:
                return session.beginDialog('hotels');
        }
    },
    // Dialog has ended
    function(session, result) {
        endConversation(session, result, 'completedSuccessfully');
    }
]).set('storage', inMemoryStorage); // Register in memory storage

bot.dialog('flights', require('./flights'));
bot.dialog('hotels', require('./hotels'));
bot.dialog('support', require('./support'))
    .triggerAction({
        matches: [/help/i, /support/i, /problem/i]
    });

bot.recognizer({
    recognize: function (context, done) {
        var intent = { score: 0.0 };
        if (context.message.text) {
            switch (context.message.text.toLowerCase()) {
                case 'help':
                    intent = { score: 1.0, intent: 'Help' };
                    break;
                case 'end':
                    intent = { score: 1.0, intent: 'End' };
                    break;
            }
        }
        done(null, intent);
    }
});

bot.dialog('helpDialog', function (session) {
    session.endDialog("This bot helps you book hotels. Flight booking is not yet implemented. Say 'End' to quit");
}).triggerAction({ matches: 'Help' });

// Add global endConversation() action bound to the 'Goodbye' intent
bot.endConversationAction('endAction', "Ok... See you later.", { matches: 'End' });

// Listen for endOfConversation activities from other sources
bot.on('endOfConversation', (message) => {
    bot.loadSession(message.address, (err, session) => {
        endConversation(session, null, 'completedSuccessfully');
    });
})

// log any bot errors into the console
bot.on('error', function (e) {
    console.log('And error ocurred', e);
});

// Code enum can be found here: https://aka.ms/codeEnum
function endConversation(session, value = null, code = null) {
    session.send('Ending conversation from the skill...');
    // Send endOfConversation with custom code and values
    const msg = {
        value,
        code,
        type: 'endOfConversation'
    };
    session.send(msg);
    // Call endConversation() to clear state
    session.endConversation();
}