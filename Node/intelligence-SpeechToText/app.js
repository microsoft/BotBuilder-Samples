/*-----------------------------------------------------------------------------
A speech to text bot for the Microsoft Bot Framework. 
-----------------------------------------------------------------------------*/

// This loads the environment variables from the .env file
require('dotenv-extended').load();

const builder = require('botbuilder'),
    fs = require('fs'),
    needle = require('needle'),
    restify = require('restify'),
    request = require('request'),
    speechService = require('./speech-service.js'),
    url = require('url');

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
// Bots Dialogs
//=========================================================

bot.dialog('/', session => {
    if (hasAudioAttachment(session)) {
        var stream = getAudioStreamFromAttachment(session.message.attachments[0]);
        speechService.getTextFromAudioStream(stream)
            .then(text => {
                session.send(processText(text));
            })
            .catch(error => {
                session.send('Oops! Something went wrong. Try again later.');
                console.error(error);
            });
    } else {
        session.send('Did you upload an audio file? I\'m more of an audible person. Try sending me a wav file');
    }
});

//=========================================================
// Utilities
//=========================================================
const hasAudioAttachment = session => {
    return session.message.attachments.length > 0 &&
        (session.message.attachments[0].contentType.indexOf('audio') === 0 ||
         session.message.attachments[0].contentType === 'video/mp4' || // FB sends audio streams as MP4
         session.message.attachments[0].contentType === 'application/octet-stream');
};

const getAudioStreamFromAttachment = attachment => {
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
};

const isSkypeAttachment = attachment => {
    if (url.parse(attachment.contentUrl).hostname.substr(-'skype.com'.length) === 'skype.com') {
        return true;
    }

    return false;
};

const processText = (text) => {
    var result = 'You said: ' + text + '.';

    if (text && text.length > 0) {
        const wordCount = text.split(' ').filter(x => x).length;
        result += '\r\n Word Count: ' + wordCount;

        const characterCount = text.replace(/ /g, '').length;
        result += '\r\n Character Count: ' + characterCount;

        const spaceCount = text.split(' ').length - 1;
        result += '\r\n Space Count: ' + spaceCount;

        const m = text.match(/[aeiou]/gi);
        const vowelCount = m === null ? 0 : m.length;
        result += '\r\n Vowel Count: ' + vowelCount;
    }

    return result;
};

//=========================================================
// Bots Events
//=========================================================

// Sends greeting message when the bot is first added to a conversation
bot.on('conversationUpdate', message => {
    if (message.membersAdded) {
        message.membersAdded.forEach(identity => {
            if (identity.id === message.address.bot.id) {
                const reply = new builder.Message()
                    .address(message.address)
                    .text('Hi! I am SpeechToText Bot. I can understand the content of any audio and convert it to text. Try sending me a wav file.');
                bot.send(reply);
                return;
            }
        });
    }
});