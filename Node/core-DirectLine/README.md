# Direct Line Bot Sample

A sample bot and a custom client communicating to each other using the Direct Line API.

[![Deploy to Azure][Deploy Button]][Deploy DirectLine/Node]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy DirectLine/Node]: https://azuredeploy.net?ptmpl=Node/DirectLine/azuredeploy.json

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering) for the instructions. Once you complete the registration, update your bot configuration with the registered config values (See [Debugging locally using ngrok](https://docs.botframework.com/en-us/node/builder/guides/core-concepts/#debugging-locally-using-ngrok) or [Deploying to Azure](https://docs.botframework.com/en-us/node/builder/guides/deploying-to-azure/#navtitle]))
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

#### Direct Line API
Credentials for the Direct Line API must be obtained from the Bot Framework developer portal, and will only allow the caller to connect to the bot for which they were generated.
In the Bot Framework developer portal, enable Direct Line in the channels list and then, configure the Direct Line secret and update its value in [DirectLineClient's app.js](DirectLineClient/app.js#L8) (`directLineSecret` variable).
Refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#channels) for more information on how to configure channels.

![Configure Direct Line](images/outcome-configure.png)

#### Publish
Also, in order to be able to run and test this sample you must [publish your bot, for example to Azure](https://docs.botframework.com/en-us/node/builder/guides/deploying-to-azure/). Alternatively, you can [Debug locally using ngrok](https://docs.botframework.com/en-us/node/builder/guides/core-concepts/#debugging-locally-using-ngrok).
Remember to update the environment variables with the `MICROSOFT_APP_ID` and `MICROSOFT_APP_PASSWORD`. If you are running the sample using Visual Studio Code, remember to update [launch.json](DirectLineBot/.vscode/launch.json#L19-L20) with the environment variables.

### Code Highlights

The Direct Line API is a simple REST API for connecting directly to a single bot. This API is intended for developers writing their own client applications, web chat controls, or mobile apps that will talk to their bot. In this sample, we are using the [Direct Line Swagger file](https://docs.botframework.com/en-us/restapi/directline/swagger.json) and [Swagger JS](https://github.com/swagger-api/swagger-js) to create a client for Node that will simplify access to the underlying REST API. Check out the client's [app.js](DirectLineClient/app.js#L8-L19) to see the client initialization.

````JavaScript
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
````

Each conversation on the Direct Line channel must be explicitly started using the `client.Conversations.Conversations_NewConversation()` function.
Check out the client's [app.js createConversation](DirectLineClient/app.js#L21-L29) function which creates a new conversation.

````JavaScript
// once the client is ready, create a new conversation 
directLineClient.then((client) => {
    client.Conversations.Conversations_NewConversation()                            // create conversation
        .then((response) => response.obj.conversationId)                            // obtain id
        .then((conversationId) => {
            sendMessagesFromConsole(client, conversationId);                        // start watching console input for sending new messages to bot
            pollMessages(client, conversationId);                                   // start polling messages from bot
        });
});
````

Once the conversation is created, a `conversationId` is returned that we can use to call other endpoints to poll or send messages.
User messages are sent to the Bot using the Direct Line Client `client.Conversations.Conversations_PostMessage` function using the `conversationId` generated in the previous step.

````JavaScript
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
````

Messages from the Bot are continually polled from the API using an interval. Check out the client's [app.js](DirectLineClient/app.js#L61-L69) usage of `client.Conversations.Conversations_GetMessages` function which retrieves conversation messages newer than the stored watermark. Messages are then filtered from anyone but our own client using the [`printMessages`](DirectLineClient/app.js#L73-L88) function.

````JavaScript
var watermark = null;
setInterval(() => {
    client.Conversations.Conversations_GetMessages({ conversationId: conversationId, watermark: watermark })
        .then((response) => {
            watermark = response.obj.watermark;                                 // use watermark so subsequent requests skip old messages 
            return response.obj.messages;
        })
        .then((messages) => printMessages(messages))
}, pollInterval);
````

The ChannelData property provides a way for you to send native metadata to take advantage of special features or concepts for a channel (see [Custom Channel Capabilities](https://docs.botframework.com/en-us/csharp/builder/sdkreference/channels.html) for more information).
Check out the [`printMessage`](DirectLineClient/app.js#L90-L104) function where the ChannelData custom content is deserialized and render appropriately.

````JavaScript
function printMessage(message) {
    console.log(message.text);
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
````

### Outcome

To run the sample, you'll need to run both Bot and Client apps.

* Running Bot app
  1. Open a CMD console and CD to sample's `DirectLineBot` directory
  2. Run `node app.js`
* Running Client app
  1. Open a CMD console and CD to sample's `DirectLineClient` directory
  2. Run `node app.js`
  
To test the ChannelData custom messages type in the Client's console `show me a hero card` or `send me a botframework image` and you should see the following outcome.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for Node and Direct Line API please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
* [Bot Framework FAQ](https://docs.botframework.com/en-us/faq/#i-have-a-communication-channel-id-like-to-be-configurable-with-bot-framework-can-i-work-with-microsoft-to-do-that)
* [Direct Line API](https://docs.botframework.com/en-us/restapi/directline/)
* [Direct Line Swagger file](https://docs.botframework.com/en-us/restapi/directline/swagger.json)
* [Swagger-JS](https://github.com/swagger-api/swagger-js)
* [Custom Channel Capabilities](https://docs.botframework.com/en-us/csharp/builder/sdkreference/channels.html)
* [Debugging locally using ngrok](https://docs.botframework.com/en-us/node/builder/guides/core-concepts/#debugging-locally-using-ngrok)
