# Carousel of Cards Bot Sample

A sample bot that sends multiple rich card attachments in a single message using the Carousel layout.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/CarouselCards]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/CarouselCards]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

You can send multiple rich card attachments in a single message. On most channels they will be sent as a list of rich cards, but some channels (like Skype and Facebook) can render them as a carousel of rich cards. The `IMessageActivity.AttachmentLayout` property allows you to control how the rich cards will be rendered. Check out the key code located in the [CarouselCardsDialog](CarouselCardsDialog.cs#L21) class where the attachments layout is changed to the Carousel mode.


> Note: Only the [Hero](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-rich-card-attachments#add-a-hero-card) and [Thumbnail](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-rich-card-attachments#add-a-thumbnail-card-to-a-message) Cards are supported for the Carousel AttachmentLayout mode.

````C#
public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
{
    var reply = context.MakeMessage();

    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
    reply.Attachments = GetCardsAttachments();

    await context.PostAsync(reply);
    
    context.Wait(this.MessageReceivedAsync);
}
````

### Outcome

You will see the following result in the Bot Framework Emulator when opening and running the sample solution. Note that an horizontal scrollbar appears, allowing you to move through the different cards.

![Sample Outcome](images/outcome-emulator.png)

You will see the following in your Facebook Messenger.

![Sample Outcome](images/outcome-facebook.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

> Note: At the time of writing this sample, there is a limit on the amount of cards that can be stacked in a carousel. For Facebook, attachments carousel is mapped to the [Generic Template](https://developers.facebook.com/docs/messenger-platform/send-api-reference/generic-template) which has a limit of 10 elements. For Skype, there's also a limit of 10 elements in the carousel.

### More Information

To get more information about how to get started in Bot Builder for .NET and Attachments please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Message Attachments Property](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-create-messages#message-attachments)
* [Add media attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments)
* [Add rich card attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments)
* [Carousel Layout in Microsoft Teams](https://msdn.microsoft.com/en-us/microsoft-teams/bots#carousel-layout)
* [RichCards sample](../cards-RichCards)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [ChannelData property](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-channeldata).
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The features used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - Microsoft Teams
> 
> They are also supported, with some limitations, in the following channels:
> - Telegram
> - DirectLine
> - WebChat
> - Slack
> - Kik
> - Email
> - GroupMe
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - SMS
