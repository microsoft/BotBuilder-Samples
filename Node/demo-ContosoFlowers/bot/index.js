var builder = require('botbuilder');
var siteUrl = require('./site-url');

var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

// Welcome Dialog
var MainOptions = {
    Shop: 'main_options_order_flowers',
    Support: 'main_options_talk_to_support'
};

var bot = new builder.UniversalBot(connector, function (session) {

    if (localizedRegex(session, [MainOptions.Shop]).test(session.message.text)) {
        // Order Flowers
        return session.beginDialog('shop:/');
    }

    var welcomeCard = new builder.HeroCard(session)
        .title('welcome_title')
        .subtitle('welcome_subtitle')
        .images([
            new builder.CardImage(session)
                .url('https://placeholdit.imgix.net/~text?txtsize=56&txt=Contoso%20Flowers&w=640&h=330')
                .alt('contoso_flowers')
        ])
        .buttons([
            builder.CardAction.imBack(session, session.gettext(MainOptions.Shop), MainOptions.Shop),
            builder.CardAction.imBack(session, session.gettext(MainOptions.Support), MainOptions.Support)
        ]);

    session.send(new builder.Message(session)
        .addAttachment(welcomeCard));
});

// Enable Conversation Data persistence
bot.set('persistConversationData', true);

// Set default locale
bot.set('localizerSettings', {
    botLocalePath: './bot/locale',
    defaultLocale: 'en'
});

// Sub-Dialogs
bot.library(require('./dialogs/shop').createLibrary());
bot.library(require('./dialogs/address').createLibrary());
bot.library(require('./dialogs/product-selection').createLibrary());
bot.library(require('./dialogs/delivery').createLibrary());
bot.library(require('./dialogs/details').createLibrary());
bot.library(require('./dialogs/checkout').createLibrary());
bot.library(require('./dialogs/settings').createLibrary());
bot.library(require('./dialogs/help').createLibrary());

// Validators
bot.library(require('./validators').createLibrary());

// Trigger secondary dialogs when 'settings' or 'support' is called
bot.use({
    botbuilder: function (session, next) {
        var text = session.message.text;

        var settingsRegex = localizedRegex(session, ['main_options_settings']);
        var supportRegex = localizedRegex(session, ['main_options_talk_to_support', 'help']);

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
bot.on('conversationUpdate', function (message) {
    if (message.membersAdded) {
        message.membersAdded.forEach(function (identity) {
            if (identity.id === message.address.bot.id) {
                bot.beginDialog(message.address, '/');
            }
        });
    }
});

// Cache of localized regex to match selection from main options
var LocalizedRegexCache = {};
function localizedRegex(session, localeKeys) {
    var locale = session.preferredLocale();
    var cacheKey = locale + ":" + localeKeys.join('|');
    if (LocalizedRegexCache.hasOwnProperty(cacheKey)) {
        return LocalizedRegexCache[cacheKey];
    }

    var localizedStrings = localeKeys.map(function (key) { return session.localizer.gettext(locale, key); });
    var regex = new RegExp('^(' + localizedStrings.join('|') + ')', 'i');
    LocalizedRegexCache[cacheKey] = regex;
    return regex;
}

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
    bot.beginDialog(address, dialogId, dialogArgs);
}

function sendMessage(message) {
    bot.send(message);
}

module.exports = {
    listen: listen,
    beginDialog: beginDialog,
    sendMessage: sendMessage
};