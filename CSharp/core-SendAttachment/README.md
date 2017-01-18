# Send Attachment Bot Sample

A sample bot that passes simple media attachments (images) to a user activity.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/SendAttachment]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/SendAttachment]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

Many messaging channels provide the ability to attach richer objects. To pass a simple media attachment (image/audio/video/file) to an activity you add a simple attachment data structure with a link to the content, setting the ContentType, ContentUrl and Name properties.
The Attachments property is an array of Attachment objects which allow you to send and receive images and other content. Check out the key code located in the [SendAttachmentDialog](SendAttachmentDialog.cs#L22-L30) class where the `replyMessage.Attachments` property of the message activity is populated with an image attachment.

````C#
public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
{
    var replyMessage = context.MakeMessage();

    // The Attachments property allows you to send and receive images and other content
    replyMessage.Attachments = new List<Attachment>()
    {
        new Attachment()
        {
            ContentUrl = "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png",
            ContentType = "image/png",
            Name = "BotFrameworkOverview.png"
        }
    };

    await context.PostAsync(replyMessage);

    context.Wait(this.MessageReceivedAsync);
}
````

### Outcome

You will see the following in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome-emulator.png)

You will see the following in your Facebook Messenger.

![Sample Outcome](images/outcome-facebook.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Attachments please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Adding Attachments to a Message](https://docs.botframework.com/en-us/core-concepts/attachments)
* [Attachments Property](https://docs.botframework.com/en-us/csharp/builder/sdkreference/activities.html#attachmentsproperty)
* [Attachments, Cards and Actions](https://docs.botframework.com/en-us/csharp/builder/sdkreference/attachments.html)
