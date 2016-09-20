# Carousel of Cards Bot Sample

A sample bot that sends multiple rich card attachments in a single message using the Carousel layout.

[![Deploy to Azure][Deploy Button]][Deploy CarouselCards/CSharp]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CarouselCards/CSharp]: https://azuredeploy.net?ptmpl=CarouselCards/CSharp/azuredeploy.json

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://aka.ms/bf-bc-emulator). Please refer to [this documentation article](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#emulator) to know more about the Bot Framework Emulator.

### Code Highlights

You can send multiple rich card attachments in a single message. On most channels they will be sent as a list of rich cards, but some channels (like Skype and Facebook) can render them as a carousel of rich cards. The `IMessageActivity.AttachmentLayout` property allows you to control how the rich cards will be rendered. Check out the key code located in the [CarouselCardsDialog](CarouselCardsDialog.cs#L21) class where the attachments layout is changed to the Carousel mode.


> Note: Only the [Hero](https://docs.botframework.com/en-us/csharp/builder/sdkreference/attachments.html#herocard) and [Thumbnail](https://docs.botframework.com/en-us/csharp/builder/sdkreference/attachments.html#thumbnailcard) Cards are supported for the Carousel AttachmentLayout mode.

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

> Note: At the time of writing this sample, there is a limit on the amount of cards that can be stacked in a carousel. For Facebook, attachments carousel is mapped to the [Generic Template](https://developers.facebook.com/docs/messenger-platform/send-api-reference/generic-template) which has a limit of 10 elements. For Skype, there's a limit of 5 elements in the carousel.

### More Information

To get more information about how to get started in Bot Builder for .NET and Attachments please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Attachments Property](https://docs.botframework.com/en-us/csharp/builder/sdkreference/activities.html#attachmentsproperty)
* [Attachments, Cards and Actions](https://docs.botframework.com/en-us/csharp/builder/sdkreference/attachments.html)
* [RichCards sample](../RichCards)
