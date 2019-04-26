# custom dialogs

Bot Framework v4 custom dialogs bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to sub-class the `Dialog` class to create different bot control mechanism like simple slot filling.

BotFramework provides a built-in base class called `Dialog`. By subclassing `Dialog`, developers can create new ways to define and control dialog flows used by the bot.

## Prerequisites

- [.NET Core SDK][4] version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/19.custom-dialogs`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/19.custom-dialogs` folder
  - Select `Custom-Dialogs.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`


## Custom Dialogs

BotFramework provides a built-in base class called `Dialog`. By subclassing Dialog, developers
can create new ways to define and control dialog flows used by the bot. By adhering to the
features of this class, developers will create custom dialogs that can be used side-by-side
with other dialog types, as well as built-in or custom prompts.

This example demonstrates a custom Dialog class called `SlotFillingDialog`, which takes a
series of "slots" which define a value the bot needs to collect from the user, as well
as the prompt it should use. The bot will iterate through all of the slots until they are
all full, at which point the dialog completes.

## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Dialog class reference][23]
- [Manage complex conversation flows with dialogs][26]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [.NET Core CLI tools][43]
- [Azure CLI][7]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]

[1]: https://dev.botframework.com
[4]: https://dotnet.microsoft.com/download
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/dialog
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-dialog-manage-complex-conversation-flow?view=azure-bot-service-4.0&tabs=javascript
[27]: https://github.com/microsoft/botbuilder-js/tree/master/libraries/botbuilder-dialogs
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[43]: https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x