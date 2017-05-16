# Get Conversation Members Bot Sample

A sample bot that retrieves the conversation's members list and detects when it changes.

[![Deploy to Azure][Deploy Button]][Deploy Node/GetConversationMembers]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/GetConversationMembers]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

#### Detecting when members leave or join a group conversation

UniversalBot triggers the `conversationUpdate` event when the bot is added to a conversation or other conversation metadata changes, including when a member joins or leaves a conversation where the bot is also present.
This event contains two properties that can be used to track when a member leaves ([`membersRemoved`](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iconversationupdate.html#membersremoved)) or joins ([`membersAdded`](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iconversationupdate.html#membersadded)) a conversation.
Both properties contains a list of [`IIdentity`](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iidentity.html), each item exposing an `id` (a channel specific ID for this identity) and a `name` property (friendly name or nickname for this identity).

````JavaScript
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
````

#### Retrieving the member's list using the Bot Connector's REST API

Currently, the Node SDK does not expose a method to retrieve the current list of members for a conversation. Alternatively, to get the list of members for a conversation, we'll make direct calls the [Bot Connector REST API](https://docs.botframework.com/en-us/restapi/connector/#!/Conversations/Conversations_GetConversationMembers).
To do this will use the Swagger Spec file and the Swagger JS client to create a client with almost no effort:  

````JavaScript
var connectorApiClient = new Swagger({
    url: 'https://raw.githubusercontent.com/Microsoft/BotBuilder/master/CSharp/Library/Microsoft.Bot.Connector.Shared/Swagger/ConnectorAPI.json',
    usePromise: true
});
````

Once a message is received in a group conversation, we'll ask the API for its members. In order to call the REST API, we need to be authenticated using the bot's JWT token (see [app.js - addTokenToClient function](app.js#L86-96)) and then override the API's hostname using the channel's serviceUrl (see [app.js - client.setHost](app.js#L41-L45)).
Then we call Swagger generated client (`client.Conversations.Conversations_GetConversationMembers`) and pass the response to a helper function that will print the members list to the conversation ([app.js - printMembersInChannel function](app.js#L98-L109)).

````JavaScript
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

    var memberList = members.map(function (m) { return '* ' + m.name + ' (Id: ' + m.id + ')'; })
        .join('\n ');

    var reply = new builder.Message()
        .address(conversationAddress)
        .text('These are the members of this conversation: \n ' + memberList);
    bot.send(reply);
}
````

### Outcome

You will see the following in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome-emulator.png)

You will see the following in Slack.

![Sample Outcome](images/outcome-slack.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

### More Information

To get more information about how to get started in Bot Builder for Node, ConversationUpdates and Bot Connector REST API please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.microsoft.com/en-us/bot-framework/nodejs/)
* [ConversationUpdate event](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.iconversationupdate.html)
* [Bot Connector REST API - GetConversationMembers](https://docs.botframework.com/en-us/restapi/connector/#!/Conversations/Conversations_GetConversationMembers)
* [Bot Connector REST API - Swagger file](https://raw.githubusercontent.com/Microsoft/BotBuilder/master/CSharp/Library/Microsoft.Bot.Connector.Shared/Swagger/ConnectorAPI.json)
* [Swagger-JS](https://github.com/swagger-api/swagger-js)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [Message.sourceEvent](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#sourceevent) method.
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The features used in this sample are fully supported in the following channels:
> - Skype
> - Slack
> - DirectLine
> - WebChat
> - Email
> - GroupMe
>
> They are also supported, with some limitations, in the following channels:
> - Facebook
> - Microsoft Teams
>
> On the other hand, they are not supported and the sample won't work as expected in the following channels:
> - SMS
> - Kik
> - Telegram
