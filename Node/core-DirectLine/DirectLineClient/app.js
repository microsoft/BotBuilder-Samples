var Swagger = require('swagger-client');
var open = require('open');
var directLineSpec = require('./directline-swagger.json');

// config items
var pollInterval = 1000;
var directLineClientName = 'DirectLineClient';
var directLineSecret = 'DIRECTLINE_SECRET';

var directLineClient = new Swagger(
    {
        spec: directLineSpec,
        usePromise: true,
    }).then((client) => {
        // add authorization header
        client.clientAuthorizations.add('AuthorizationBotConnector', new Swagger.ApiKeyAuthorization('Authorization', 'BotConnector ' + directLineSecret, 'header'));
        return client;
    }).catch((err) =>
        console.error('Error initializing DirectLine client', err));

// once the client is ready, create a new conversation 
directLineClient.then((client) => {
    client.Conversations.Conversations_NewConversation()                            // create conversation
        .then((response) => response.obj.conversationId)                            // obtain id
        .then((conversationId) => {
            sendMessagesFromConsole(client, conversationId);                        // start watching console input for sending new messages to bot
            pollMessages(client, conversationId);                                   // start polling messages from bot
        });
});

// Read from console (stdin) and send input to conversation using DirectLine client
function sendMessagesFromConsole(client, conversationId) {
    var stdin = process.openStdin();
    process.stdout.write('Command> ');
    stdin.addListener('data', function (e) {
        var input = e.toString().trim();
        if (input) {
            // exit
            if (input.toLowerCase() === 'exit') {
                return process.exit();
            }

            // send message
            client.Conversations.Conversations_PostMessage(
            {
                conversationId: conversationId,
                message: {
                    from: directLineClientName,
                    text: input
                }
            }).catch((err) => console.error('Error sending message:', err));

            process.stdout.write('Command> ');
        }
    });
}

// Poll Messages from conversation using DirectLine client
function pollMessages(client, conversationId) {
    console.log('Starting polling message for conversationId: ' + conversationId);
    var watermark = null;
    setInterval(() => {
        client.Conversations.Conversations_GetMessages({ conversationId: conversationId, watermark: watermark })
            .then((response) => {
                watermark = response.obj.watermark;                                 // use watermark so subsequent requests skip old messages 
                return response.obj.messages;
            })
            .then((messages) => printMessages(messages))
    }, pollInterval);
}

// Helpers methods
function printMessages(messages) {
    if (messages && messages.length) {
        // ignore own messages
        messages = messages.filter((m) => m.from !== directLineClientName);

        if (messages.length) {
            process.stdout.clearLine();
            process.stdout.cursorTo(0);

            // print other messages
            messages.forEach(printMessage);

            process.stdout.write('Command> ');
        }
    };
}

function printMessage(message) {
    if(message.text) {
        console.log(message.text);
    }
    
    if (message.channelData) {
        switch (message.channelData.contentType) {
            case "application/vnd.microsoft.card.hero":
                renderHeroCard(message.channelData);
                break;

            case "image/png":
                console.log('Opening the requested image ' + message.channelData.contentUrl);
                open(message.channelData.contentUrl);
                break;
        }
    }
}

function renderHeroCard(channelData) {
    var width = 70;
    var contentLine = (content) =>
        ' '.repeat((width - content.length) / 2) +
        content +
        ' '.repeat((width - content.length) / 2);

    console.log('/' + '*'.repeat(width + 1));
    console.log('*' + contentLine(channelData.content.title) + '*');
    console.log('*' + ' '.repeat(width) + '*');
    console.log('*' + contentLine(channelData.content.text) + '*');
    console.log('*'.repeat(width + 1) + '/');
}