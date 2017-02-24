/*-----------------------------------------------------------------------------
A Similar Products bot for the Microsoft Bot Framework. 
-----------------------------------------------------------------------------*/

// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder'),
    restify = require('restify'),
    request = require('request').defaults({ encoding: null }),
    url = require('url'),
    validUrl = require('valid-url'),
    imageService = require('./image-service');

// Maximum number of hero cards to be returned in the carousel. If this number is greater than 10, skype throws an exception.
var MAX_CARD_COUNT = 10;

//=========================================================
// Bot Setup
//=========================================================

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

server.post('/api/messages', connector.listen());

// Gets the similar images by checking the type of the image (stream vs URL) and calling the appropriate image service method.
var bot = new builder.UniversalBot(connector, function (session) {
    if (hasImageAttachment(session)) {
        var stream = getImageStreamFromMessage(session.message);
        imageService
            .getSimilarProductsFromStream(stream)
            .then(function (visuallySimilarProducts) { handleApiResponse(session, visuallySimilarProducts); })
            .catch(function (error) { handleErrorResponse(session, error); });
    } else {
        var imageUrl = parseAnchorTag(session.message.text) || (validUrl.isUri(session.message.text) ? session.message.text : null);
        if (imageUrl) {
            imageService
                .getSimilarProductsFromUrl(imageUrl)
                .then(function (visuallySimilarProducts) { handleApiResponse(session, visuallySimilarProducts); })
                .catch(function (error) { handleErrorResponse(session, error); });
        } else {
            session.send('Did you upload an image? I\'m more of a visual person. Try sending me an image or an image URL');
        }
    }
});

//=========================================================
// Bots Events
//=========================================================

//Sends greeting message when the bot is first added to a conversation
bot.on('conversationUpdate', function (message) {
    if (message.membersAdded) {
        message.membersAdded.forEach(function (identity) {
            if (identity.id === message.address.bot.id) {
                var reply = new builder.Message()
                    .address(message.address)
                    .text('Hi! I am SimilarProducts Bot. I can find you similar products. Try sending me an image or an image URL.');
                bot.send(reply);
            }
        });
    }
});

//=========================================================
// Utilities
//=========================================================

function hasImageAttachment(session) {
    return session.message.attachments.length > 0 &&
        session.message.attachments[0].contentType.indexOf('image') !== -1;
}

function getImageStreamFromMessage(message) {
    var headers = {};
    var attachment = message.attachments[0];
    if (checkRequiresToken(message)) {
        // The Skype attachment URLs are secured by JwtToken,
        // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
        // https://github.com/Microsoft/BotBuilder/issues/662
        connector.getAccessToken(function (error, token) {
            var tok = token;
            headers['Authorization'] = 'Bearer ' + token;
            headers['Content-Type'] = 'application/octet-stream';

            return request.get({ url: attachment.contentUrl, headers: headers });
        });
    }

    headers['Content-Type'] = attachment.contentType;
    return request.get({ url: attachment.contentUrl, headers: headers });
}

function checkRequiresToken(message) {
    return message.source === 'skype' || message.source === 'msteams';
}

/**
 * Gets the href value in an anchor element.
 * Skype transforms raw urls to html. Here we extract the href value from the url
 * @param {string} input Anchor Tag
 * @return {string} Url matched or null
 */
function parseAnchorTag(input) {
    var match = input.match("^<a href=\"([^\"]*)\">[^<]*</a>$");
    if (match && match[1]) {
        return match[1];
    }

    return null;
}

//=========================================================
// Response Handling
//=========================================================

function handleApiResponse(session, images) {
    if (images && images.constructor === Array && images.length > 0) {

        var productCount = Math.min(MAX_CARD_COUNT, images.length);

        var cards = new Array();
        for (var i = 0; i < productCount; i++) {
            cards.push(constructCard(session, images[i]));
        }

        // create reply with Carousel AttachmentLayout
        var reply = new builder.Message(session)
            .text('Here are some visually similar products I found')
            .attachmentLayout(builder.AttachmentLayout.carousel)
            .attachments(cards);
        session.send(reply);
    } else {
        session.send('Couldn\'t find similar products images for this one');
    }
}

function constructCard(session, image) {
    return new builder.HeroCard(session)
        .title(image.name)
        .subtitle(image.hostPageDisplayUrl)
        .images([
            builder.CardImage.create(session, image.thumbnailUrl)
        ])
        .buttons([
            builder.CardAction.openUrl(session, image.hostPageUrl, 'Buy from merchant'),
            builder.CardAction.openUrl(session, image.webSearchUrl, 'Find more in Bing')
        ]);
}

function handleErrorResponse(session, error) {
    session.send('Oops! Something went wrong. Try again later.');
    console.error(error);
}
