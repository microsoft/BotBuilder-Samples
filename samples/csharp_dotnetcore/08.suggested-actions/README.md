This sample demonstrates the use of suggested actions.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking, and artificial intelligence techniques well-integrated with existing business services.

## Suggested actions
Suggested actions enable your bot to present buttons that the user can tap to provide input. Suggested actions appear close to the composer and enhance
user experience by enabling the user to answer a question or make a selection with a simple tap of a button, rather than having to type a response with
a keyboard. Unlike buttons that appear within rich cards (which remain visible and accessible to the user even after being tapped), buttons that appear
within the suggested actions pane will disappear after the user makes a selection. This prevents the user from tapping stale buttons within a conversation
and simplifies bot development (since you will not need to account for that scenario).

## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/08.suggested-actions` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally

## Visual studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/08.suggested-actions`) and open `SuggestedActionsBot.csproj` in Visual Studio
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/08.suggested-actions`
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
- [Bot basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics)
- [Channels and Bot Connector service](https://docs.microsoft.com/azure/bot-service/bot-concepts)
- [Activity processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing)
- [Suggested actions](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-add-suggested-actions?view=azure-bot-service-4.0&tabs=csharp#suggest-action-using-button)
