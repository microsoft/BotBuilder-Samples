# Direct Line Bot Sample

A sample bot and a custom client communicating to each other using the Direct Line API.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/DirectLine]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/DirectLine]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.microsoft.com/en-us/bot-framework/portal-register-bot) for the instructions. Once you complete the registration, update the [Bot's Web.config](DirectLineBot/Web.config#L9-L11) file with the registered config values (Bot Id, MicrosoftAppId and MicrosoftAppPassword)

#### Direct Line API
Credentials for the Direct Line API must be obtained from the Bot Framework developer portal, and will only allow the caller to connect to the bot for which they were generated.
In the Bot Framework developer portal, enable Direct Line in the channels list and then, configure the Direct Line secret and update its value in the [client's App.config](DirectLineClient/App.config#L4-L5) file alongside with the Bot Id. Make sure that the checkbox for version 3.0 [PREVIEW] is checked. Refer to [this](https://docs.microsoft.com/en-us/bot-framework/portal-configure-channels) for more information on how to configure channels.

![Configure Direct Line](images/outcome-configure.png)

#### Publish
Also, in order to be able to run and test this sample you must [publish your bot, for example to Azure](https://docs.microsoft.com/en-us/bot-framework/publish-bot-overview). Alternatively, you can use [Ngrok to interact with your local bot in the cloud](https://blogs.msdn.microsoft.com/jamiedalton/2016/07/29/ms-bot-framework-ngrok/). 

### Code Highlights

The Direct Line API is a simple REST API for connecting directly to a single bot. This API is intended for developers writing their own client applications, web chat controls, or mobile apps that will talk to their bot. The [Direct Line v3.0 Nuget package](https://www.nuget.org/packages/Microsoft.Bot.Connector.DirectLine/3.0.0-beta) simplifies access to the underlying REST API.

Each conversation on the Direct Line channel must be explicitly started using the `DirectLineClient.Conversations.StartConversationAsync`.
Check out the client's [Program.cs](DirectLineClient/Program.cs#L25-L27) class which creates a new `DirectLineClient` and starts a new conversation.


````C#
DirectLineClient client = new DirectLineClient(directLineSecret);
            
var conversation = await client.Conversations.StartConversationAsync();
````

User messages are sent to the Bot using the Direct Line Client `Conversations.PostActivityAsync` method using the `ConversationId` generated in the previous step.

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
            Activity userMessage = new Activity
            {
                From = new ChannelAccount(fromUser),
                Text = input,
                Type = ActivityTypes.Message
            };

            await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
        }
    }
}
````

Messages from the Bot are continually polled from the API in another Thread in the `ReadBotMessagesAsync` method. Check out the [Program.cs](DirectLineClient/Program.cs#L64-L69) usage of `GetActivitiesAsync` method which retrieves conversation messages newer than the stored watermark. Activities are then filtered to receive messages from the Bot only.

````C#
var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
watermark = activitySet?.Watermark;

var activities = from x in activitySet.Activities
                    where x.From.Id == botId
                    select x;
````

DirectLine v3.0 (unlike version 1.1) has support for Attachments (see [Add media attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments) for more information about attachments). Check out the `ReadBotMessagesAsync` method in [Program.cs](DirectLineClient/Program.cs#L75-L92) to see how the Attachments are retrieved and rendered appropriately based on their type.


````C#
if (activity.Attachments != null)
{
    foreach (Attachment attachment in activity.Attachments)
    {
        switch (attachment.ContentType)
        {
            case "application/vnd.microsoft.card.hero":
                RenderHeroCard(attachment);
                break;

            case "image/png":
                Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");

                Process.Start(attachment.ContentUrl);
                break;
        }
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

To test the Attachments type `show me a hero card` or `send me a botframework image` and you should see the following outcome.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Bot Framework FAQ](https://docs.microsoft.com/en-us/bot-framework/resources-bot-framework-faq#i-have-a-communication-channel-id-like-to-be-configurable-with-bot-framework-can-i-work-with-microsoft-to-do-that)
* [Direct Line API - v3.0](https://docs.botframework.com/en-us/restapi/directline3/)
* [Direct Line v3.0 Nuget package](https://www.nuget.org/packages/Microsoft.Bot.Connector.DirectLine/3.0.0-beta)
* [Add media attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments)
* [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started)
