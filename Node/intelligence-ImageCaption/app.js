/*-----------------------------------------------------------------------------
An image caption bot for the Microsoft Bot Framework. 
-----------------------------------------------------------------------------*/

// This loads the environment variables from the .env file
require('dotenv-extended').load();

const builder = require('botbuilder'),
    captionService = require('./caption-service'),
    needle = require("needle"),
    restify = require('restify'),
    url = require('url');
    validUrl = require('valid-url');

//=========================================================
// Bot Setup
//=========================================================

// Setup Restify Server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3979, () => {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot
const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());


//=========================================================
// Bots Events
//=========================================================

//Sends greeting message when the bot is first added to a conversation
bot.on('conversationUpdate', message => {
    if (message.membersAdded) {
        message.membersAdded.forEach(identity => {
            if (identity.id === message.address.bot.id) {
                const reply = new builder.Message()
                    .address(message.address)
                    .text("Hi! I am ImageCaption Bot. I can understand the content of any image and try to describe it as well as any human. Try sending me an image or an image URL.");
                bot.send(reply);
            }
        });
    }
});


//=========================================================
// Bots Dialogs
//=========================================================

// Gets the caption by checking the type of the image (stream vs URL) and calling the appropriate caption service method.
bot.dialog('/', session => {
    if (hasImageAttachment(session)) {
        var stream = getImageStreamFromUrl(session.message.attachments[0]);
        captionService
            .getCaptionFromStream(stream)
            .then(caption => handleSuccessResponse(session, caption))
            .catch(error => handleErrorResponse(session, error));
    }
    else if(imageUrl = (parseAnchorTag(session.message.text) || (validUrl.isUri(session.message.text)? session.message.text : null))) {
        captionService
            .getCaptionFromUrl(imageUrl)
            .then(caption => handleSuccessResponse(session, caption))
            .catch(error => handleErrorResponse(session, error));
    }
    else {
        session.send("Did you upload an image? I'm more of a visual person. Try sending me an image or an image URL");
    }
});

//=========================================================
// Utilities
//=========================================================
const hasImageAttachment = session => {
    return ((session.message.attachments.length > 0) && (session.message.attachments[0].contentType.indexOf("image") !== -1));
}

const getImageStreamFromUrl = attachment => {
    var headers = {};
    if (isSkypeAttachment(attachment)) {
        // The Skype attachment URLs are secured by JwtToken,
        // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
        // https://github.com/Microsoft/BotBuilder/issues/662
        connector.getAccessToken((error, token) => {
            var tok = token;
            headers['Authorization'] = 'Bearer ' + token;
            headers['Content-Type'] = 'application/octet-stream';

            return needle.get(attachment.contentUrl, { headers: headers });
        });
    }

    headers['Content-Type'] = attachment.contentType;
    return needle.get(attachment.contentUrl, { headers: headers });
}

const isSkypeAttachment = attachment => {
    if (url.parse(attachment.contentUrl).hostname.substr(-"skype.com".length) == "skype.com") {
        return true;
    }

    return false;
}

/**
 * Gets the href value in an anchor element.
 * Skype transforms raw urls to html. Here we extract the href value from the url
 */
const parseAnchorTag = input => {
    var match = input.match("^<a href=\"([^\"]*)\">[^<]*</a>$");
    if(match && match[1]) {
        return match[1];
    }

    return null;
}

//=========================================================
// Response Handling
//=========================================================
const handleSuccessResponse = (session, caption) => {
    if (caption) {
        session.send("I think it's " + caption);
    }
    else {
        session.send("Couldn't find a caption for this one");
    }

}

const handleErrorResponse = (session, error) => {
    session.send("Oops! Something went wrong. Try again later.");
    console.error(error);
}
