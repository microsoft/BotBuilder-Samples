var builder = require('botbuilder');
var siteUrl = require('./site-url');

var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector, { persistUserData: true });

// Welcome Dialog
const MainOptions = {
    Shop: 'Order flowers',
    Support: 'Talk to support'
};
bot.dialog('/', (session) => {
    if(session.message.text.trim().toUpperCase() === MainOptions.Shop.toUpperCase()) {
        // Order Flowers
        return session.beginDialog('shop:/');
    }

    var welcomeCard = new builder.HeroCard(session)
        .title('Welcome to the Contoso Flowers')
        .subtitle('These are the flowers you are looking for!')
        .images([
            new builder.CardImage(session)
                .url('https://placeholdit.imgix.net/~text?txtsize=56&txt=Contoso%20Flowers&w=640&h=330')
                .alt('Contoso Flowers')
        ])
        .buttons([
            builder.CardAction.imBack(session, MainOptions.Shop, MainOptions.Shop),
            builder.CardAction.imBack(session, MainOptions.Support, MainOptions.Support)
        ]);

    session.send(new builder.Message(session)
        .addAttachment(welcomeCard));
});

// Sub-Dialogs
bot.library(require('./dialogs/shop'));
bot.library(require('./dialogs/address'));
bot.library(require('./dialogs/product-selection'));
bot.library(require('./dialogs/delivery'));
bot.library(require('./dialogs/details'));
bot.library(require('./dialogs/checkout'));
bot.library(require('./dialogs/settings'));
bot.library(require('./dialogs/help'));

// Validators
bot.library(require('./validators'));

// Trigger secondary dialogs when 'settings' or 'support' is called
const settingsRegex = /^settings/i;
const supportRegex = new RegExp('^(' + MainOptions.Support + '|help)', 'i');
bot.use({
    botbuilder: (session, next) => {
        var text = session.message.text;
        if (settingsRegex.test(text)) {
            // interrupt and trigger 'settings' dialog 
            return session.beginDialog('settings:/');
        } else if (supportRegex.test(text)) {
            // interrupt and trigger 'help' dialog
            return session.beginDialog('help:/');
        }   

        // continue normal flow
        next();
    }
});

// Send welcome when conversation with bot is started, by initiating the root dialog
bot.on('conversationUpdate', (message) => {
    if (message.membersAdded) {
        message.membersAdded.forEach((identity) => {
            if (identity.id === message.address.bot.id) {
                bot.beginDialog(message.address, '/');
            }
        });
    }
});

// Connector listener wrapper to capture site url
var connectorListener = connector.listen();
function listen() {
    return function (req, res) {
        // Capture the url for the hosted application
        // We'll later need this url to create the checkout link 
        var url = req.protocol + '://' + req.get('host');
        siteUrl.save(url);
        connectorListener(req, res);
    };
}

// Other wrapper functions
function beginDialog(address, dialogId, dialogArgs) {
    bot.beginDialog(address, dialogId, dialogArgs)
}

function sendMessage(message) {
    bot.send(message);
}

module.exports = {
    listen: listen,
    beginDialog: beginDialog,
    sendMessage: sendMessage
};