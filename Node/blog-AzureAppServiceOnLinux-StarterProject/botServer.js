/*
    @author Nils Whitmont <nils.whitmont@gmail.com>

    @summary Slack Bot example for MS Bot Framework
 */
'use strict';

// INIT NODE MODULES
const botBuilder = require('botbuilder');
const botServer = require('restify').createServer();


botServer.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`${botServer.name} listening to: ${botServer.url}`);
});

// lets us know our bot is online when visiting the deployment URL in browser
botServer.get('/', function (request, response, next) {
    response.send({ status: 'online' });
    next();
});

// Create chatConnector for communicating with the Bot Framework Service
const chatConnector = new botBuilder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

// Listen for messages from users
botServer.post('/api/messages', chatConnector.listen());

// Create your bot with a function to receive messages from the user
const bot = new botBuilder.UniversalBot(chatConnector);

bot.dialog('/', [
    function (session) {
        session.send('Hi, I am Slack Support Bot!\n\n I will show you what is possible with Bot Framework for Slack.');
        botBuilder.Prompts.choice(session, 'Choose a demo', ['Hero Card', 'Slack Buttons', 'Basic Text']);
    },
    function (session, result) {
        // start dialog per user choice
        switch (result.response.entity) {
            case 'Hero Card':
                session.beginDialog('herocard');
                break;
            case 'Slack Buttons':
                session.beginDialog('buttons');
                break;
            case 'Basic Text':
                session.beginDialog('basicMessage');
                break;
            default:
                session.send('invalid choice');
                break;
        }
    }
]);

bot.dialog('herocard', [
    function (session) {
        var heroCard = new botBuilder.Message(session)
            .textFormat(botBuilder.TextFormat.xml)
            .attachments([
                new botBuilder.HeroCard(session)
                    .title("Hero Card")
                    .subtitle("Space Needle")
                    .text("The Space Needle is an observation tower in Seattle, Washington, a landmark of the Pacific Northwest, and an icon of Seattle.")
                    .images([
                        botBuilder.CardImage.create(session, "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/Seattlenighttimequeenanne.jpg/320px-Seattlenighttimequeenanne.jpg")
                    ])
                    .tap(botBuilder.CardAction.openUrl(session, "https://en.wikipedia.org/wiki/Space_Needle"))
            ]);
        session.endDialog(heroCard);
    }
]);

bot.dialog('buttons', [
    function (session) {
        session.send('What kind of sandwich would you like?');
        botBuilder.Prompts.choice(session, "Choose one:", ["Wasabi Tuna", "Grinder", "Veggie Special"]);
    },
    function (session, result) {
        session.endDialog(`You picked: ${result.response.entity}`)
    }
]).triggerAction({ matches: /button/i });

bot.dialog('basicMessage', function (session) {
    session.endDialog('A basic message');
});

// trigger action allows the user to exit in the middle of any dialog
bot.dialog('exit', function (session) {
    session.endConversation('Goodbye!');
}).triggerAction({ matches: /exit/i });

// END OF LINE
