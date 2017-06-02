# Cards as Attachments Sample Bot

A sample bot that renders several types of cards as attachments, while also showing the generated JSON for each one of these cards at the message's payload, and the C# and NodeJs code required to build them.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/CardAttachments]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/CardAttachments]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.

### Code Highlights

Many messaging channels provide the ability to attach richer objects. The Bot Framework has the ability to render rich cards as attachments. There are several types of cards supported: Hero Card, Thumbnail Card, Receipt Card, Sign-In Card, Animation Card, Video Card and Audio Card. Once the desired Card type is selected, it is mapped into an `Attachment` data structure. Check out the key code located in the [RichCardScorable](public-TestBot/Scorables/RichCards/RichCardScorable.cs#L31) class where the `message.Attachments` property of the message activity is assigned with a list of card attachments got from the `GetCardAttachments` abstract method overriden at each descendant of this class.

````C#
protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
{
    await base.PostAsync(item, state, token);

    var message = this.BotToUser.MakeMessage();

    message.Attachments = this.GetCardAttachments();

    var sample = RichCardsSample;
    var text = RichCardsText;

    // should display carousel?
    if (message.Attachments.Count() > 1)
    {
        message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        sample = CarouselSample;
        text = CarouselText;
    }

    await this.BotToUser.PostAsync(message);

    var moreMessage = this.BotToUser.MakeMessage();

    moreMessage.Text = $"To know more about {text} check these [C#]({CSharpSamplesRoot + sample}) & [NodeJs]({NodeJsSamplesRoot + sample}) samples. Type **{Constants.JsonTrigger}** to view attachment details;  or type **{Constants.CSharpTrigger}** or **{Constants.NodeJsTrigger}** for the source code that generated this card.";

    await this.BotToUser.PostAsync(moreMessage);
}
````

The core of the sample is implemented following a variety of the [Command pattern](https://en.wikipedia.org/wiki/Command_pattern) by using scorables. For these 'commands' there is defined a base [TriggerScorable](public-TestBot/Scorables/TriggerScorable.cs) class which has a [Trigger](public-TestBot/Scorables/TriggerScorable.cs#L21) property that allows the invoker knowing if the command should be executed. Within the sample, the key actors of this pattern are mapped to the following:

* Command: Represented by each non-abstract scorable class in the solution having the `Trigger` value overriden.
* Receiver: On each command itself, represented by the logic performed at the `PostAsync` method override from the scorable base.
* Invoker: The bot builder framework scorable evaluation logic that processes a set of scorable instances, evaluates them in order to get the best one, and if it is any dispatchs it.
* Client: The bot builder framework by knowing the registered IScorable<IActivity, double> types, in order to instance them, and provide those instances to the scorable evaluation logic in order to dispatch the best result (which would be the `invoker`).

Once a message is received, the [Conversation.SendAsync](public-TestBot/Controllers/MessagesController.cs#L24) will go through all these registered scorables first, and it will dispatch the corresponding one (if that is the case). Otherwise it will continue to the default [RootDialog](public-TestBot/RootDialog.cs) made root there that provides a helping message to the user. In order to know if the scorable should be dispatched the bot framework interacts with a few methods from the IScorable interface which are: `PrepareAsync`, `HasScore`, `GetScore`, and within our solution these are implemented here at the [TriggerScorable](public-TestBot/Scorables/TriggerScorable.cs#L28-L48) class. As you can see, the [PrepareAsync](public-TestBot/Scorables/TriggerScorable.cs#L47) method implementation searches if the command's `Trigger` keyword is contained within the received message text, while the other two methods mentioned above are relying on the `state` returned by this one in order to notify the bot framework it should proceed with the `PostAsync` dispatching.

````C#
protected override Task<bool> PrepareAsync(IActivity item, CancellationToken token)
{
    var message = item.AsMessageActivity();

    if (message == null)
    {
        return Task.FromResult(false);
    }

    return Task.FromResult(item != null ? message.Text.ToLowerInvariant().Contains(this.Trigger.ToLowerInvariant()) : false);
}
````

All these 'commands' are registered at [Global.asax](public-TestBot/Global.asax.cs#L31-L38) by using reflection to search for all non-abstract [TriggerScorable](public-TestBot/Scorables/TriggerScorable.cs) descendants. The trigger list, made by the `Trigger` of each one of them, defines all the available keywords mapped to commands in the bot.

#### Hero Card

The Hero card is a multipurpose card; it primarily hosts a single large image, a button, and a "tap action", along with text content to display on the card. Check out the `GetCardAttachments` method in the [HeroCardScorable ](public-TestBot/Scorables/RichCards/HeroCardScorable.cs#L21-L34) class for a Hero Card sample.

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new HeroCard
        {
            Title = "BotFramework Hero Card",
            Subtitle = "Your bots — wherever your users are talking",
            Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
            Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
        }.ToAttachment()
    };
}
````

#### Thumbnail Card
The Thumbnail card is a multipurpose card; it primarily hosts a single small image, a button, and a "tap action", along with text content to display on the card. Check out the `GetCardAttachments` method in the [ThumbnailCardScorable](public-TestBot/Scorables/RichCards/ThumbnailCardScorable.cs#L21-L34) class for a Thumbnail Card sample.

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new ThumbnailCard
        {
            Title = "BotFramework Thumbnail Card",
            Subtitle = "Your bots — wherever your users are talking",
            Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
            Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
        }.ToAttachment()
    };
}
````

#### Receipt Card
The receipt card allows the Bot to present a receipt to the user. Check out the `GetCardAttachments` method in the [ReceiptCardScorable](public-TestBot/Scorables/RichCards/ReceiptCardScorable.cs#L21-L46) class for a Receipt Card sample.

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new ReceiptCard
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
        }.ToAttachment()
    };
}
````

#### Sign-In Card
The Sign-In card is a card representing a request to sign in the user. Check out the `GetCardAttachments` method in the [SigninCardScorable](public-TestBot/Scorables/RichCards/SigninCardScorable.cs#L21-L31) class for a Sign-In Card sample.

> Note: The sign in card can be used to initiate an authentication flow which is beyond this sample. For a complete authentication flow sample take a look at [AuthBot](https://github.com/MicrosoftDX/AuthBot).

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new SigninCard
        {
            Text = "BotFramework Sign-in Card",
            Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
        }.ToAttachment()
    };
}
````

#### Animation Card
The Animation card is a card that’s capable of playing animated GIFs or short videos. Check out the `GetCardAttachments` method in the [AnimationCardScorable](public-TestBot/Scorables/RichCards/AnimationCardScorable.cs#L21-L42) class for an Animation Card sample.

````C#
private static Attachment GetAnimationCard()
{
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new AnimationCard
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
        }.ToAttachment()
    };
}
````

> Note: At the time of writing this sample, Skype requires the Animation card to have a Thumbnail Url.

#### Video Card
The Video card is a card that’s capable of playing videos. Check out the `GetCardAttachments` method in the [VideoCardScorable](public-TestBot/Scorables/RichCards/VideoCardScorable.cs#L21-L52) class for a Video Card sample.

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new VideoCard
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
        }.ToAttachment()
    };
}
````

> Note: At the time of writing this sample, Skype requires the Video card to have a Thumbnail Url.

#### Audio Card
The Audio card is a card that’s capable of playing an audio file. Check out the `GetCardAttachments` method in the [AudioCardScorable](public-TestBot/Scorables/RichCards/AudioCardScorable.cs#L21-L52) class for an Audio Card sample.

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new AudioCard
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
        }.ToAttachment()
    };
}
````

> Note: At the time of writing this sample, Skype requires the Audio card to have a Thumbnail Url.

#### Carousel of Cards

You can send multiple rich card attachments in a single message. On most channels they will be sent as a list of rich cards, but some channels (like Skype and Facebook) can render them as a carousel of rich cards. The `IMessageActivity.AttachmentLayout` property allows you to control how the rich cards will be rendered. Check out the key code located in the [RichCardScorable](public-TestBot/Scorables/RichCards/RichCardScorable.cs#L36-L42) class where the attachments layout is changed to the Carousel mode (ie. if more than 1 attachment are found at the returned list, the `AttachmentLayoutTypes.Carousel` value is being set) using the attachments returned by `GetCardAttachments` override at [CarouselCardsScorable](public-TestBot/Scorables/RichCards/CarouselCardsScorable.cs#L21-L84).

````C#
protected override IList<Attachment> GetCardAttachments()
{
    return new List<Attachment>
    {
        new HeroCard
        {
            Title = "BotFramework Hero Card",
            Subtitle = "Your bots — wherever your users are talking",
            Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
            Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
        }.ToAttachment(),
        new ThumbnailCard
        {
            Title = "BotFramework Thumbnail Card",
            Subtitle = "Your bots — wherever your users are talking",
            Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
            Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
        }.ToAttachment(),
        new AnimationCard
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
        }.ToAttachment(),
        new VideoCard
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
        }.ToAttachment()
    };
}
````

#### Special commands

There are three commands that allows you to see information related with the lastest card sample shown. From showing the code in C# or NodeJs on how to build that card, to showing the JSON payload within the message returned to the channel containing the serialized form of the card as an attachment.

These commands are the following:

* JSON: shows the JSON serialized payload for the latest card sample shown (implemented by [ShowJsonScorable](public-TestBot/Scorables/Miscellaneous/ShowJsonScorable.cs)).
* C#: shows the C# code to build the latest card sample shown (implemented by [ShowCSharpScorable](public-TestBot/Scorables/Miscellaneous/ShowCSharpScorable.cs)).
* Node: shows the NodeJs code to build the latest card sample shown (implemented by [ShowNodeJsScorable](public-TestBot/Scorables/Miscellaneous/ShowNodeJsScorable.cs)).

As you have might seen all these commands inherit from a base [ShowPrivateDataScorable](public-TestBot/Scorables/Miscellaneous/ShowPrivateDataScorable.cs) class which has logic to get stored information within the bot-user conversation private data. There, the logic at the [PostAsync](public-TestBot/Scorables/Miscellaneous/ShowPrivateDataScorable.cs#L20-L31) override uses the abstract `DataKey` property value to retrieve the proper information regarding the instance of the command implementation it is executing.

````C#
protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
{
    var data = default(string);

    this.BotData.PrivateConversationData.TryGetValue(this.DataKey, out data);

    var reply = this.BotToUser.MakeMessage();

    reply.Text = string.IsNullOrWhiteSpace(data) ? this.NotAvailableMessage : string.Format(CodeTemplate, data);

    await this.BotToUser.PostAsync(reply);
}
````

In order to obtain the information required by each one of these commands, the sample is storing relevant information within the bot-user private conversation data, and to fulfill this action there are two different things implemented:

* An abstract command named [ExtractCodeScorable](public-TestBot/Scorables/ExtractCodeScorable.cs) that should be inherited by commands showing samples and have C# & NodeJs code associated to the generation of these (samples). As you can see within its implementation the `SaveCode` private method is storing the respective command's code at the [conversation private data](public-TestBot/Scorables/ExtractCodeScorable.cs#L39).
* An `IActivityLogger` implementation that, once registered at the bot framework conversation pipeline, will intercept all input/output activities the bot processes. This interface is implemented by the [ActivityLogger](public-TestBot/ActivityLogger.cs) class which is storing the [JSON](public-TestBot/ActivityLogger.cs#L28) payload from the attachments of the bot's responses when displaying cards.

### Outcome

Within the sample there is included a chatting page that allows you to execute all the commands detailed above. This page embedds a web chat control and requires you to configure the following [web.config](public-TestBot/Web.config#L9-L12) settings for the bot as it uses direct line back channel to interact with it:

````XML
  <appSettings>
    <!-- TODO: update these with public/registered BotId, Microsoft App Id and your Microsoft App Password-->
    <add key="BotId" value="Bot" />
    <add key="MicrosoftAppId" value="" />
    <add key="MicrosoftAppPassword" value="" />
    <add key="DirectLineSecret" value="" />
  </appSettings>
````

At that view you will see the following while interacting with the bot running different commands from the sample.

#### Hero Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Hero Card](images/hero-card.png)|![Hero Card JSON payload](images/hero-json.png)|![Hero Card C# code](images/hero-csharp.png)|![Hero Card NodeJs code](images/hero-nodejs.png)|

#### Thumbnail Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Thumbnail Card](images/thumbnail-card.png)|![Thumbnail Card JSON payload](images/thumbnail-json.png)|![Thumbnail Card C# code](images/thumbnail-csharp.png)|![Thumbnail Card NodeJs code](images/thumbnail-nodejs.png)|

#### Receipt Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Receipt Card](images/receipt-card.png)|![Receipt Card JSON payload](images/receipt-json.png)|![Receipt Card C# code](images/receipt-csharp.png)|![Receipt Card NodeJs code](images/receipt-nodejs.png)|

#### Sign-In Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Sign-In Card](images/signin-card.png)|![Sign-In Card JSON payload](images/signin-json.png)|![Sign-In Card C# code](images/signin-csharp.png)|![Sign-In Card NodeJs code](images/signin-nodejs.png)|

#### Animation Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Animation Card](images/animation-card.png)|![Animation Card JSON payload](images/animation-json.png)|![Animation Card C# code](images/animation-csharp.png)|![Animation Card NodeJs code](images/animation-nodejs.png)|

#### Video Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Video Card](images/video-card.png)|![Video Card JSON payload](images/video-json.png)|![Video Card C# code](images/video-csharp.png)|![Video Card NodeJs code](images/video-nodejs.png)|

#### Audio Card

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Audio Card](images/audio-card.png)|![Audio Card JSON payload](images/audio-json.png)|![Audio Card C# code](images/audio-csharp.png)|![Audio Card NodeJs code](images/audio-nodejs.png)|

#### Carousel of Cards

| Card sample | JSON | C# | NodeJs |
|-------------|------|----|--------|
|![Sample Outcome Carousel of Cards](images/carousel-card.png)|![Carousel of Cards JSON payload](images/carousel-json.png)|![Carousel of Cards C# code](images/carousel-csharp.png)|![Carousel of Cards NodeJs code](images/carousel-nodejs.png)|

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
