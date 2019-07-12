// This loads the environment variables from the .env file
require('dotenv-extended').load();

var builder = require('botbuilder');
var restify = require('restify');
var Promise = require('bluebird');
var url = require('url');
var Swagger = require('swagger-client');
var connectorSpec = require('./connector-api-swagger.json');

// Swagger client for Bot Connector API
var connectorApiClient = new Swagger({
    spec: connectorSpec,
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

// Bot Storage: Here we register the state storage for your bot. 
// Default store: volatile in-memory store - Only for prototyping!
// We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
// For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
var inMemoryStorage = new builder.MemoryBotStorage();

// Bot setup
var bot = new builder.UniversalBot(connector, function (session) {
    var message = session.message;
    var conversationId = message.address.conversation.id;

    // when a group conversation message is recieved,
    // get the conversation members using the REST API and print it on the conversation.

    // 1. inject the JWT from the connector to the client on every call
    addTokenToClient(connector, connectorApiClient).then(function (client) {
        // 2. override API client host and schema (https://api.botframework.com) with channel's serviceHost (e.g.: https://slack.botframework.com or http://localhost:NNNN)
        var serviceUrl = url.parse(message.address.serviceUrl);
        var serviceScheme = serviceUrl.protocol.split(':')[0];
        client.setSchemes([serviceScheme]);
        client.setHost(serviceUrl.host);
        client.setBasePath(serviceUrl.path);
        // 3. GET /v3/conversations/{conversationId}/members
        return client.Conversations.Conversations_GetConversationMembers({ conversationId: conversationId })
            .then(function (res) {
                printMembersInChannel(message.address, res.obj);
            });
    }).catch(function (error) {
        console.log('Error retrieving conversation members', error);
    });
}).set('storage', inMemoryStorage); // Register in memory storage

bot.on('conversationUpdate', function (message) {
    if (message.membersAdded && message.membersAdded.length > 0) {
        var membersAdded = message.membersAdded
            .map(function (m) {
                var isSelf = m.id === message.address.bot.id;
                return (isSelf ? message.address.bot.name : m.name) || '' + ' (Id: ' + m.id + ')';
            })
            .join(', ');

        bot.send(new builder.Message()
            .address(message.address)
            .text('Welcome ' + membersAdded));
    }

    if (message.membersRemoved && message.membersRemoved.length > 0) {
        var membersRemoved = message.membersRemoved
            .map(function (m) {
                var isSelf = m.id === message.address.bot.id;
                return (isSelf ? message.address.bot.name : m.name) || '' + ' (Id: ' + m.id + ')';
            })
            .join(', ');

        bot.send(new builder.Message()
            .address(message.address)
            .text('The following members ' + membersRemoved + ' were removed or left the conversation :('));
    }
});

// Helper methods

// Inject the connector's JWT token into to the Swagger client
function addTokenToClient(connector, clientPromise) {
    // ask the connector for the token. If it expired, a new token will be requested to the API
    var obtainToken = Promise.promisify(connector.getAccessToken.bind(connector));
    return Promise.all([obtainToken(), clientPromise]).then(function (values) {
        var token = values[0];
        var client = values[1];
        client.clientAuthorizations.add('AuthorizationBearer', new Swagger.ApiKeyAuthorization('Authorization', 'Bearer ' + token, 'header'));
        return client;
    });
}

// Create a message with the member list and send it to the conversationAddress
function printMembersInChannel(conversationAddress, members) {
    if (!members || members.length === 0) return;

    const memberList = members.map((member) => {
        const memberName = member.name || '';
        const memberId = member.id || '';
        return `* ${memberName} Id: ${memberId}`;
    }).join('\n ');

    const reply = new builder.Message()
        .address(conversationAddress)
        .text('These are the members of this conversation: \n ' + memberList);
    bot.send(reply);
}