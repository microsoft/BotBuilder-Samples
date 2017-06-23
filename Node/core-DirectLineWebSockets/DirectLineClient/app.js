var Swagger = require('swagger-client');
var open = require('open');
var rp = require('request-promise');

// Config settings
var directLineSecret = 'DIRECTLINE_SECRET';

// directLineUserId is the field that identifies which user is sending activities to the Direct Line service.
// Because this value is created and sent within your Direct Line client, your bot should not
// trust the value for any security-sensitive operations. Instead, have the user log in and
// store any sign-in tokens against the Conversation or Private state fields. Those fields
// are secured by the conversation ID, which is protected with a signature.
var directLineUserId = 'DirectLineClient';

var useW3CWebSocket = false;
process.argv.forEach(function (val, index, array) {
    if (val === 'w3c') {
        useW3CWebSocket = true;
    }
});

var directLineSpecUrl = 'https://docs.botframework.com/en-us/restapi/directline3/swagger.json';
var directLineClient = rp(directLineSpecUrl)
    .then(function (spec) {
        // Client
        return new Swagger({
            spec: JSON.parse(spec.trim()),
            usePromise: true
        });
    })
    .then(function (client) {
        // Obtain a token using the Direct Line secret
        return rp({
            url: 'https://directline.botframework.com/v3/directline/tokens/generate',
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + directLineSecret
            },
            json: true            
        }).then(function (response) {
            // Then, replace the client's auth secret with the new token
            var token = response.token;
            client.clientAuthorizations.add('AuthorizationBotConnector', new Swagger.ApiKeyAuthorization('Authorization', 'Bearer ' + token, 'header'));
            return client;            
        });
    })
    .catch(function (err) {
        console.error('Error initializing DirectLine client', err);
        throw err;
    });

// Once the client is ready, create a new conversation 
directLineClient.then(function (client) {
    client.Conversations.Conversations_StartConversation()
        .then(function (response) {
            var responseObj = response.obj;

            // Start console input loop from stdin
            sendMessagesFromConsole(client, responseObj.conversationId);

            if (useW3CWebSocket) {
                // Start receiving messages from WS stream - using W3C client
                startReceivingW3CWebSocketClient(responseObj.streamUrl, responseObj.conversationId);
            } else {
                // Start receiving messages from WS stream - using Node client
                startReceivingWebSocketClient(responseObj.streamUrl, responseObj.conversationId);
            }
        });
});

// Read from console (stdin) and send input to conversation using DirectLine client
function sendMessagesFromConsole(client, conversationId) {
    var stdin = process.openStdin();
    process.stdout.write('Command> ');
    stdin.addListener('data', function (e) {
        var input = e.toString().trim();
        if (input) {
            if (input.toLowerCase() === 'exit') {
                return process.exit();
            }

            // Send message
            client.Conversations.Conversations_PostActivity(
                {
                    conversationId: conversationId,
                    activity: {
                        textFormat: 'plain',
                        text: input,
                        type: 'message',
                        from: {
                            id: directLineUserId,
                            name: directLineUserId
                        }
                    }
                }).catch(function (err) {
                    console.error('Error sending message:', err);
                });

            process.stdout.write('Command> ');
        }
    });
}

function startReceivingWebSocketClient(streamUrl, conversationId) {
    console.log('Starting WebSocket Client for message streaming on conversationId: ' + conversationId);

    var ws = new (require('websocket').client)();

    ws.on('connectFailed', function (error) {
        console.log('Connect Error: ' + error.toString());
    });

    ws.on('connect', function (connection) {
        console.log('WebSocket Client Connected');
        connection.on('error', function (error) {
            console.log("Connection Error: " + error.toString());
        });
        connection.on('close', function () {
            console.log('WebSocket Client Disconnected');
        });
        connection.on('message', function (message) {
            // Occasionally, the Direct Line service sends an empty message as a liveness ping
            // Ignore these messages
            if (message.type === 'utf8' && message.utf8Data.length > 0) {
                var data = JSON.parse(message.utf8Data);
                printMessages(data.activities);
                // var watermark = data.watermark;
            }
        });
    });

    ws.connect(streamUrl);
}

function startReceivingW3CWebSocketClient(streamUrl, conversationId) {
    console.log('Starting W3C WebSocket Client for message streaming on conversationId: ' + conversationId);

    var ws = new (require('websocket').w3cwebsocket)(streamUrl);

    ws.onerror = function () {
        console.log('Connection Error');
    };

    ws.onopen = function () {
        console.log('W3C WebSocket Client Connected');
    };

    ws.onclose = function () {
        console.log('W3C WebSocket Client Disconnected');
    };

    ws.onmessage = function (e) {
        // Occasionally, the Direct Line service sends an empty message as a liveness ping
        // Ignore these messages
        if (typeof e.data === 'string' && e.data.length > 0) {
            var data = JSON.parse(e.data);
            printMessages(data.activities);
            // var watermark = data.watermark;
        }
    };
}

// Helpers methods
function printMessages(activities) {
    if (activities && activities.length) {
        // Ignore own messages
        activities = activities.filter(function (m) { return m.from.id !== directLineUserId });

        if (activities.length) {
            process.stdout.clearLine();
            process.stdout.cursorTo(0);

            // Print other messages
            activities.forEach(printMessage);

            process.stdout.write('Command> ');
        }
    }
}

function printMessage(activity) {
    if (activity.text) {
        console.log(activity.text);
    }

    if (activity.attachments) {
        activity.attachments.forEach(function (attachment) {
            switch (attachment.contentType) {
                case "application/vnd.microsoft.card.hero":
                    renderHeroCard(attachment);
                    break;

                case "image/png":
                    console.log('Opening the requested image ' + attachment.contentUrl);
                    open(attachment.contentUrl);
                    break;
            }
        });
    }
}

function renderHeroCard(attachment) {
    var width = 70;
    var contentLine = function (content) {
        return ' '.repeat((width - content.length) / 2) +
            content +
            ' '.repeat((width - content.length) / 2);
    }

    console.log('/' + '*'.repeat(width + 1));
    console.log('*' + contentLine(attachment.content.title) + '*');
    console.log('*' + ' '.repeat(width) + '*');
    console.log('*' + contentLine(attachment.content.text) + '*');
    console.log('*'.repeat(width + 1) + '/');
}