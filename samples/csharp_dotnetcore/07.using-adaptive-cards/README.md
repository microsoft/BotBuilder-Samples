This sample demonstrates the use of Adaptive Cards.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Adaptive Cards
Card authors describe their content as a simple JSON object. That content can then be rendered natively inside a host application,
automatically adapting to the look and feel of the host. For example, Contoso Bot can author an Adaptive Card through the Bot Framework,
and when delivered to Skype, it will look and feel like a Skype card. When that same payload is sent to Microsoft Teams, it will look
and feel like Microsoft Teams. As more host apps start to support Adaptive Cards, that same payload will automatically light up inside
these applications, yet still feel entirely native to the app. Users win because everything feels familiar. Host apps win because they
control the user experience. Card authors win because their content gets broader reach without any additional work.
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards`) and open AdaptiveCardsBot.csproj in Visual Studio
- Run the project (press `F5` key).

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards`.
- Type `dotnet run`.

# Deploy this bot to Azure

You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction)
- [Bot basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/azure/bot-service/bot-concepts)
- [Activity processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing)
- [Adaptive Cards](https://adaptivecards.io/)

