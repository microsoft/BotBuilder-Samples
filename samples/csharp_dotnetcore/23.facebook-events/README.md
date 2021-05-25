# Facebook Events

Bot Framework v4 facebook events bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), is shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies and opt-in events.  Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

More information about configuring a bot for Facebook Messenger can be found here: [Connect a bot to Facebook](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook)

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- Enable Facebook Channel
    The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy,
    and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-3.0).

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/23.facebook-events`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/23.facebook-events` folder
  - Select `Facebook-Events-Bot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Facebook Quick Replies](https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/0)
- [Facebook PostBack](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/)
- [Facebook Opt-in](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
