// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');
var Swagger = require('swagger-client');
var Promise = require('bluebird');
var url = require('url');
var fs = require('fs');
var util = require('util');

// Swagger client for Bot Connector API
var connectorApiClient = new Swagger(
    {
        url: 'https://raw.githubusercontent.com/Microsoft/BotBuilder/master/CSharp/Library/Microsoft.Bot.Connector.Shared/Swagger/ConnectorAPI.json',
        usePromise: true
    });

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

// Listen for messages
server.post('/api/messages', connector.listen());

// Bot Dialogs
var bot = new builder.UniversalBot(connector, [
    function (session) {
        session.send('Welcome, here you can see attachment alternatives:');
        builder.Prompts.choice(session, 'What sample option would you like to see?', Options, {
            maxRetries: 3
        });
    },
    function (session, results) {
        var option = results.response ? results.response.entity : Inline;
        switch (option) {
            case Inline:
                return sendInline(session, './images/small-image.png', 'image/png', 'BotFrameworkLogo.png');
            case Upload:
                return uploadFileAndSend(session, './images/big-image.png', 'image/png', 'BotFramework.png');
            case External:
                var url = 'https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png';
                return sendInternetUrl(session, url, 'image/png', 'BotFrameworkOverview.png');
        }
    }]);

var Inline = 'Show inline attachment';
var Upload = 'Show uploaded attachment';
var External = 'Show Internet attachment';
var Options = [Inline, Upload, External];

// Sends attachment inline in base64
function sendInline(session, filePath, contentType, attachmentFileName) {
    fs.readFile(filePath, function (err, data) {
        if (err) {
            return session.send('Oops. Error reading file.');
        }

        var base64 = Buffer.from(data).toString('base64');

        var msg = new builder.Message(session)
            .addAttachment({
                contentUrl: util.format('data:%s;base64,%s', contentType, base64),
                contentType: contentType,
                name: attachmentFileName
            });

        session.send(msg);
    });
}

// Uploads a file using the Connector API and sends attachment
function uploadFileAndSend(session, filePath, contentType, attachmentFileName) {

    // read file content and upload
    fs.readFile(filePath, function (err, data) {
        if (err) {
            return session.send('Oops. Error reading file.');
        }

        // Upload file data using helper function
        uploadAttachment(
            data,
            contentType,
            attachmentFileName,
            connector,
            connectorApiClient,
            session.message.address.serviceUrl,
            session.message.address.conversation.id)
            .then(function (attachmentUrl) {
                // Send Message with Attachment obj using returned Url
                var msg = new builder.Message(session)
                    .addAttachment({
                        contentUrl: attachmentUrl,
                        contentType: contentType,
                        name: attachmentFileName
                    });

                session.send(msg);
            })
            .catch(function (err) {
                console.log('Error uploading file', err);
                session.send('Oops. Error uploading file. ' + err.message);
            });
    });
}

// Sends attachment using an Internet url
function sendInternetUrl(session, url, contentType, attachmentFileName) {
    var msg = new builder.Message(session)
        .addAttachment({
            contentUrl: url,
            contentType: contentType,
            name: attachmentFileName
        });

    session.send(msg);
}

// Uploads file to Connector API and returns Attachment URLs
function uploadAttachment(fileData, contentType, fileName, connector, connectorApiClient, baseServiceUrl, conversationId) {

    var base64 = Buffer.from(fileData).toString('base64');

    // Inject the connector's JWT token into to the Swagger client
    function addTokenToClient(connector, clientPromise) {
        // ask the connector for the token. If it expired, a new token will be requested to the API
        var obtainToken = Promise.promisify(connector.addAccessToken.bind(connector));
        var options = {};
        return Promise.all([clientPromise, obtainToken(options)]).then(function (values) {
            var client = values[0];
            var hasToken = !!options.headers.Authorization;
            if (hasToken) {
                var authHeader = options.headers.Authorization;
                client.clientAuthorizations.add('AuthorizationBearer', new Swagger.ApiKeyAuthorization('Authorization', authHeader, 'header'));
            }

            return client;
        });
    }

    // 1. inject the JWT from the connector to the client on every call
    return addTokenToClient(connector, connectorApiClient).then(function (client) {
        // 2. override API client host and schema (https://api.botframework.com) with channel's serviceHost (e.g.: https://slack.botframework.com or http://localhost:NNNN)
        var serviceUrl = url.parse(baseServiceUrl);
        var serviceScheme = serviceUrl.protocol.split(':')[0];
        client.setSchemes([serviceScheme]);
        client.setHost(serviceUrl.host);

        // 3. POST /v3/conversations/{conversationId}/attachments
        var uploadParameters = {
            conversationId: conversationId,
            attachmentUpload: {
                type: contentType,
                name: fileName,
                originalBase64: base64
            }
        };

        return client.Conversations.Conversations_UploadAttachment(uploadParameters)
            .then(function (res) {
                var attachmentId = res.obj.id;
                var attachmentUrl = serviceUrl;

                attachmentUrl.pathname = util.format('/v3/attachments/%s/views/%s', attachmentId, 'original');
                return attachmentUrl.format();
            });
    });
}
