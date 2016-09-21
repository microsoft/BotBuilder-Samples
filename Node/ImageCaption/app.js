/*-----------------------------------------------------------------------------
An image caption bot for the Microsoft Bot Framework. 
-----------------------------------------------------------------------------*/

// This loads the environment variables from the .env file
require('dotenv-extended').load();

if (!process.env.MICROSOFT_VISION_API_KEY) {
    console.error("Missing MICROSOFT_VISION_API_KEY. Please set it in the '.env' file. You can obtain one from https://www.microsoft.com/cognitive-services/en-us/subscriptions?productId=/products/54d873dd5eefd00dc474a0f4");
    process.exit()
}

const builder = require('botbuilder'),
    captionService = require('./caption-service'),
    needle = require("needle"),
    restify = require('restify'),
    validUrl = require('valid-url');

//=========================================================
// Bot Setup
//=========================================================

// Setup Restify Server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
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
        var stream = needle.get(session.message.attachments[0].contentUrl);        
        captionService
            .getCaptionFromStream(stream)
            .then(caption => handleSuccessResponse(session, caption))
            .catch(error => handleErrorResponse(session, error));
    }
    else if (validUrl.isUri(session.message.text)) {
        captionService
            .getCaptionFromUrl(session.message.text)
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
