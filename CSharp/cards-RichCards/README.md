# Rich Cards Bot Sample

A sample bot to renders several types of cards as attachments.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/RichCards]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/RichCards]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

Many messaging channels provide the ability to attach richer objects. The Bot Framework has the ability to render rich cards as attachments. There are several types of cards supported: Hero Card, Thumbnail Card, Receipt Card, Sign-In Card, Animation Card, Video Card and Audio Card. Once the desired Card type is selected, it is mapped into an `Attachment` data structure. Check out the key code located in the [CardsDialog](CardsDialog.cs#L46-L51) class where the `message.Attachments` property of the message activity is populated with a card attachment.

````C#
public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
{
    var selectedCard = await result;

    var message = context.MakeMessage();

    var attachment = GetSelectedCard(selectedCard);
    message.Attachments.Add(attachment);

    await context.PostAsync(message);

    context.Wait(this.MessageReceivedAsync);
}
````

#### Hero Card

The Hero card is a multipurpose card; it primarily hosts a single large image, a button, and a "tap action", along with text content to display on the card. Check out the `GetHeroCard` method in the [CardsDialog](CardsDialog.cs#L80-L92) class for a Hero Card sample.

````C#
private static Attachment GetHeroCard()
{
    var heroCard = new HeroCard
    {
        Title = "BotFramework Hero Card",
        Subtitle = "Your bots — wherever your users are talking",
        Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
        Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
    };

    return heroCard.ToAttachment();
}
````

#### Thumbnail Card
The Thumbnail card is a multipurpose card; it primarily hosts a single small image, a button, and a "tap action", along with text content to display on the card. Check out the `GetThumbnailCard` method in the [CardsDialog](CardsDialog.cs#L94-L106) class for a Thumbnail Card sample.

````C#
private static Attachment GetThumbnailCard()
{
    var heroCard = new ThumbnailCard
    {
        Title = "BotFramework Thumbnail Card",
        Subtitle = "Your bots — wherever your users are talking",
        Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
        Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
    };

    return heroCard.ToAttachment();
}
````

#### Receipt Card
The receipt card allows the Bot to present a receipt to the user. Check out the `GetReceiptCard` method in the [CardsDialog](CardsDialog.cs#L108-L132) class for a Receipt Card sample.

````C#
private static Attachment GetReceiptCard()
{
    var receiptCard = new ReceiptCard
    {
        Title = "John Doe",
        Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
        Items = new List<ReceiptItem>
        {
            new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
            new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
        },
        Tax = "$ 7.50",
        Total = "$ 90.95",
        Buttons = new List<CardAction>
        {
            new CardAction(
                ActionTypes.OpenUrl,
                "More information",
                "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
                "https://azure.microsoft.com/en-us/pricing/")
        }
    };

    return receiptCard.ToAttachment();
}
````

#### Sign-In Card
The Sign-In card is a card representing a request to sign in the user. Check out the `GetSigninCard` method in the [CardsDialog](CardsDialog.cs#L134-L143) class for a Sign-In Card sample.

> Note: The sign in card can be used to initiate an authentication flow which is beyond this sample. For a complete authentication flow sample take a look at [AuthBot](https://github.com/MicrosoftDX/AuthBot).

````C#
private static Attachment GetSigninCard()
{
    var signinCard = new SigninCard
    {
        Text = "BotFramework Sign-in Card",
        Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
    };

    return signinCard.ToAttachment();
}
````

#### Animation Card
The Animation card is a card that’s capable of playing animated GIFs or short videos. Check out the `GetAnimationCard` method in the [CardsDialog](CardsDialog.cs#L145-L165) class for an Animation Card sample.

````C#
private static Attachment GetAnimationCard()
{
    var animationCard = new AnimationCard
    {
        Title = "Microsoft Bot Framework",
        Subtitle = "Animation Card",
        Image = new ThumbnailUrl
        {
            Url = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png"
        },
        Media = new List<MediaUrl>
        {
            new MediaUrl()
            {
                Url = "http://i.giphy.com/Ki55RUbOV5njy.gif"
            }
        }
    };

    return animationCard.ToAttachment();
}
````

> Note: At the time of writing this sample, Skype requires the Animation card to have a Thumbnail Url.

#### Video Card
The Video card is a card that’s capable of playing videos. Check out the `GetVideoCard` method in the [CardsDialog](CardsDialog.cs#L167-L197) class for a Video Card sample.

````C#
private static Attachment GetVideoCard()
{
    var videoCard = new VideoCard
    {
        Title = "Big Buck Bunny",
        Subtitle = "by the Blender Institute",
        Text = "Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation's previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.",
        Image = new ThumbnailUrl
        {
            Url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg"
        },
        Media = new List<MediaUrl>
        {
            new MediaUrl()
            {
                Url = "http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4"  
            }
        },
        Buttons = new List<CardAction>
        {
            new CardAction()
            {
                Title = "Learn More",
                Type = ActionTypes.OpenUrl,
                Value = "https://peach.blender.org/"
            }
        }
    };

    return videoCard.ToAttachment();
}
````

> Note: At the time of writing this sample, Skype requires the Video card to have a Thumbnail Url.

#### Audio Card
The Audio card is a card that’s capable of playing an audio file. Check out the `GetAudioCard` method in the [CardsDialog](CardsDialog.cs#L199-L229) class for an Audio Card sample.

````C#
private static Attachment GetAudioCard()
{
    var audioCard = new AudioCard
    {
        Title = "I am your father",
        Subtitle = "Star Wars: Episode V - The Empire Strikes Back",
        Text = "The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving as executive producer. The second installment in the original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
        Image = new ThumbnailUrl
        {
            Url = "https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"
        },
        Media = new List<MediaUrl>
        {
            new MediaUrl()
            {
                Url = "http://www.wavlist.com/movies/004/father.wav"
            }
        },
        Buttons = new List<CardAction>
        {
            new CardAction()
            {
                Title = "Read More",
                Type = ActionTypes.OpenUrl,
                Value = "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back"
            }
        }
    };

    return audioCard.ToAttachment();
}
````

> Note: At the time of writing this sample, Skype requires the Audio card to have a Thumbnail Url.

### Outcome

You will see the following in the Bot Framework Emulator, Facebook Messenger and Skype when opening and running the sample.

#### Hero Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Hero Card](images/outcome-hero-emulator.png)|![Sample Outcome Hero Card](images/outcome-hero-facebook.png)|![Sample Outcome Hero Card](images/outcome-hero-skype.png)|

#### Thumbnail Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-emulator.png)|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-facebook.png)|![Sample Outcome Thumbnail Card](images/outcome-thumbnail-skype.png)|

#### Receipt Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Receipt Card](images/outcome-receipt-emulator.png)|![Sample Outcome Receipt Card](images/outcome-receipt-facebook.png)|![Sample Outcome Receipt Card](images/outcome-receipt-skype.png)|

#### Sign-In Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Sign-In Card](images/outcome-signin-emulator.png)|![Sample Outcome Sign-In Card](images/outcome-signin-facebook.png)|![Sample Outcome Sign-In Card](images/outcome-signin-skype.png)|

#### Animation Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Animation Card](images/outcome-animation-emulator.png)|![Sample Outcome Animation Card](images/outcome-animation-facebook.png)|![Sample Outcome Animation Card](images/outcome-animation-skype.png)|

#### Video Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Video Card](images/outcome-video-emulator.png)|![Sample Outcome Video Card](images/outcome-video-facebook.png)|![Sample Outcome Video Card](images/outcome-video-skype.png)|

#### Audio Card

| Emulator | Facebook | Skype |
|----------|-------|----------|
|![Sample Outcome Audio Card](images/outcome-audio-emulator.png)|![Sample Outcome Audio Card](images/outcome-audio-facebook.png)|![Sample Outcome Audio Card](images/outcome-audio-skype.png)|


### More Information

To get more information about how to get started in Bot Builder for .NET and Attachments please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Message Attachments Property](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-create-messages#message-attachments)
* [Add media attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments)
* [Add rich card attachments to messages](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-media-attachments)
* [Cards and buttons on Microsoft Teams](https://msdn.microsoft.com/en-us/microsoft-teams/bots#cards-and-buttons)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [ChannelData property](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-channeldata).
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The Hero card and Thumbnail card used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - Telegram
> - DirectLine
> - WebChat
> - Slack
> - Email
> - GroupMe
> 
> They are also supported, with some limitations, in the following channel:
> - Kik
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - SMS
>
> The Receipt card and Sign-in card used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - Telegram
> - DirectLine
> - WebChat
> - Slack
> - Email
> - GroupMe
> 
> They are also supported, with some limitations, in the following channel:
> - Kik
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - SMS
> - Microsoft Teams
>
> The Animation card, Video card and Audio card used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - WebChat
>
> They are also supported, with some limitations, in the following channel:
> - Slack (Only Animation card is supported)
> - GroupMe
> - Telegram (Only Animation and Audio cards are supported)
> - Email
>
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - Microsoft Teams
> - Kik
> - SMS
>