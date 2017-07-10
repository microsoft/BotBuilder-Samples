# Roller Skill Bot Sample

A sample bot optimized for speech-enabled channels such as Cortana.

[![Deploy to Azure][Deploy Button]][Deploy CSharp/ContosoFlowers]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/ContosoFlowers]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* To fully test this sample you must:
    * Register you bot in [Microsoft Bot Framework Portal](https://dev.botframework.com/bots). Please refer to [this](https://docs.microsoft.com/en-us/bot-framework/portal-register-bot) for the instructions. Once you complete the registration, update the [Bot's Web.config](Web.config#L9-L11) file with the registered config values (Bot Id, MicrosoftAppId and MicrosoftAppPassword). 
    * Enable the Cortana Channel and register your bot as a Cortana skill. Refer to [this](https://docs.microsoft.com/en-us/bot-framework/portal-configure-channels) for more information on how to configure channels and to [this](https://docs.microsoft.com/en-us/bot-framework/channels/channel-cortana) and [this](https://docs.microsoft.com/en-us/cortana/tutorials/bot-skills/add-bot-to-cortana-channel) for specific information on how to add the Cortana channel to your bot.
    *  [Publish your bot, for example to Azure](https://docs.microsoft.com/en-us/bot-framework/deploy-bot-overview) or use [Ngrok to interact with your local bot in the cloud](https://blogs.msdn.microsoft.com/jamiedalton/2016/07/29/ms-bot-framework-ngrok/).


### Code Highlights
Many channels provides an audio component besides the usual visual component, allowing your bot to have a voice. The BotBuilder SDK has set of features designed specifically to support speech based channels such as Cortana. 

The [`IMessageActivity`](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Connector.Shared/IMessageActivity.cs) interface contains two properties (`Speak` and `InputHint`) to support speech responses allowing you to define what the bot will say and the speech based client should manage the microphone.

The `Speak` property should contain text or Speech Synthesis Markup Language (SSML). 

The `InputHint` property expects one of the following values from the [`InputHints`](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Connector.Shared/InputHints.cs) enumeration:

|Name|Description|
|---|---|
|AcceptingInput|Your bot is passively ready for input but is not waiting on a response (the mic should be closed)|
|ExpectingInput|Your bot is actively expecting a response from the user (the mic should be left open)|
|IgnoringInput|Your bot is ignoring input. Bots may send this hint if they are actively processing a request and will ignore input from users until the request is complete|

In general the BotBuilder SDK will send these hints for you automatically, so you don't have to worry too much about them. Checkout the use of the `Speak` and `InputHint` properties in the [`StartAsync`](Dialogs/HelpDialog.cs#L20-L21) method from the [`HelpDialog`](Dialogs/HelpDialog.cs) class.

```
public async Task StartAsync(IDialogContext context)
{
    var message = context.MakeMessage();
    message.Speak = SSMLHelper.Speak(Resources.HelpSSML);
    message.InputHint = InputHints.AcceptingInput;

    message.Attachments = new List<Attachment>
    {
        new HeroCard(Resources.HelpTitle)
        {
            Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.ImBack, "Roll Dice", value: RollDiceOptionValue),
                new CardAction(ActionTypes.ImBack, "Play Craps", value: PlayCrapsOptionValue)
            }
        }.ToAttachment()
    };

    await context.PostAsync(message);

    context.Done<object>(null);
}
```

Built-in prompts also have speech support and they can send plain text as well as SSML thanks to the `Speak` and `RetrySpeak` properties from the [`PromptOptions<T>`](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs#L90) class. Checkout the use of the `Speak` property in a PromptChoice dialog in the [`StartAsync`](Dialogs/CreateGameDialog.cs#L31) method from the [`CreateGameDialog`](Dialogs/CreateGameDialog.cs) class.

```
public async Task StartAsync(IDialogContext context)
{
    context.UserData.SetValue<GameData>(Utils.GameDataKey, new GameData());

    var descriptions = new List<string>() { "4 Sides", "6 Sides", "8 Sides", "10 Sides", "12 Sides", "20 Sides" };
    var choices = new Dictionary<string, IReadOnlyList<string>>()
        {
        { "4", new List<string> { "four", "for", "4 sided", "4 sides" } },
        { "6", new List<string> { "six", "sex", "6 sided", "6 sides" } },
        { "8", new List<string> { "eight", "8 sided", "8 sides" } },
        { "10", new List<string> { "ten", "10 sided", "10 sides" } },
        { "12", new List<string> { "twelve", "12 sided", "12 sides" } },
        { "20", new List<string> { "twenty", "20 sided", "20 sides" } }
    };

    var promptOptions = new PromptOptions<string>(
        Resources.ChooseSides,
        choices: choices,
        descriptions: descriptions,
        speak: SSMLHelper.Speak(Utils.RandomPick(Resources.ChooseSidesSSML)));

    PromptDialog.Choice(context, this.DiceChoiceReceivedAsync, promptOptions);
}
```
### Outcome
You will see the following when talking to the Bot via Cortana.

![Sample Outcome](images/outcome-cortana.png)


### More Information
To get more information about how to get started in Bot Builder for .NET and Attachments please review the following resources:
* [Bot Builder for .NET](https://docs.microsoft.com/en-us/bot-framework/dotnet/)
* [Build a speech-enabled bot with Cortana Skills](https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-cortana-skill)
* [The Cortana Skills Kit](https://docs.microsoft.com/en-us/cortana/getstarted)
* [Creating a Skill from Scratch using Bot Framework](https://docs.microsoft.com/en-us/cortana/tutorials/bot-skills/add-bot-to-cortana-channel)
