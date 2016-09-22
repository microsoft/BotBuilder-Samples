# Direct Line Bot Sample

A sample bot and a custom client communicating to each other using the Direct Line API.

[![Deploy to Azure][Deploy Button]][Deploy DirectLine/CSharp]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy DirectLine/CSharp]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering) for the instructions. Once you complete the registration, update the [Bot's Web.config](DirectLineBot/Web.config#L9-L11) file with the registered config values (Bot Id, MicrosoftAppId and MicrosoftAppPassword)

#### Direct Line API
Credentials for the Direct Line API must be obtained from the Bot Framework developer portal, and will only allow the caller to connect to the bot for which they were generated.
In the Bot Framework developer portal, enable Direct Line in the channels list and then, configure the Direct Line secret and update its value in the [client's App.config](DirectLineClient/App.config#L4-L5) file alongside with the Bot Id. Refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#channels) for more information on how to configure channels.

![Configure Direct Line](images/outcome-configure.png)

#### Publish
Also, in order to be able to run and test this sample you must [publish your bot, for example to Azure](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#publishing). Alternatively, you can use [Ngrok to interact with your local bot in the cloud](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#using-the-emulator-with-ngrok-to-interact-with-your-bot-in-the-cloud). 

### Code Highlights

The Direct Line API is a simple REST API for connecting directly to a single bot. This API is intended for developers writing their own client applications, web chat controls, or mobile apps that will talk to their bot. The [Direct Line Nuget package](https://www.nuget.org/packages/Microsoft.Bot.Connector.DirectLine) simplifies access to the underlying REST API.

Each conversation on the Direct Line channel must be explicitly started using the `DirectLineClient.Conversations.NewConversationAsync`.
Check out the client's [Program.cs](DirectLineClient/Program.cs#L26-L28) class which creates a new `DirectLineClient` and starts a new conversation.


````C#
DirectLineClient client = new DirectLineClient(directLineSecret);
            
var conversation = await client.Conversations.NewConversationAsync();
````

User messages are sent to the Bot using the Direct Line Client `Conversations.PostMessageAsync` method using the `ConversationId` generated in the previous step.

````C#
while (true)
{
    string input = Console.ReadLine().Trim();

    if (input.ToLower() == "exit")
    {
        break;
    }
    else
    {
        if (input.Length > 0)
        {
            Message userMessage = new Message
            {
                FromProperty = fromUser,
                Text = input
            };

            await client.Conversations.PostMessageAsync(conversation.ConversationId, userMessage);
        }
    }
}
````

Messages from the Bot are continually polled from the API in another Thread in the `ReadBotMessagesAsync` method. Check out the [Program.cs](DirectLineClient/Program.cs#L64-L69) usage of `GetMessagesAsync` method which retrieves conversation messages newer than the stored watermark. Messages are then filtered to receive messages from the Bot only.

````C#
var messages = await client.Conversations.GetMessagesAsync(conversationId, watermark);
watermark = messages?.Watermark;

var messagesFromBotText = from x in messages.Messages
                    where x.FromProperty == botId
                    select x;
````

The ChannelData property provides a way for you to send native metadata to take advantage of special features or concepts for a channel (see [Custom Channel Capabilities](https://docs.botframework.com/en-us/csharp/builder/sdkreference/channels.html) for more information.) Check out the `ReadBotMessagesAsync` method in [Program.cs](DirectLineClient/Program.cs#L75-L91) to where the ChannelData custom content is deserialized and render appropriately.


````C#
if (message.ChannelData != null)
{
    var channelData = JsonConvert.DeserializeObject<DirectLineChannelData>(message.ChannelData.ToString());

    switch (channelData.ContentType)
    {
        case "application/vnd.microsoft.card.hero":
            RenderHeroCard(channelData);
            break;

        case "image/png":
            Console.WriteLine($"Opening the requested image '{channelData.ContentUrl}'");

            Process.Start(channelData.ContentUrl);
            break;
    }  
}
````


### Outcome

To run the sample, you'll need to run both Bot and Client apps.
* Running Bot app
    1. In the Visual Studio Solution Explorer window, right click on the **DirectLineBot** project.
    2. In the contextual menu, select Debug, then Start New Instance and wait for the _Web application_ to start.
* Running Client app
    1. In the Visual Studio Solution Explorer window, right click on the **DirectLineSampleClient** project.
    2. In the contextual menu, select Debug, then Start New Instance and wait for the _Console application_ to start.

To test the ChannelData custom messages type `show me a hero card` or `send me a botframework image` and you should see the following outcome.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Bot Framework FAQ](https://docs.botframework.com/en-us/faq/#i-have-a-communication-channel-id-like-to-be-configurable-with-bot-framework-can-i-work-with-microsoft-to-do-that)
* [Direct Line API](https://docs.botframework.com/en-us/restapi/directline/)
* [Direct Line Nuget package](https://www.nuget.org/packages/Microsoft.Bot.Connector.DirectLine)
* [Custom Channel Capabilities](https://docs.botframework.com/en-us/csharp/builder/sdkreference/channels.html)
* [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#using-the-emulator-with-ngrok-to-interact-with-your-bot-in-the-cloud)
