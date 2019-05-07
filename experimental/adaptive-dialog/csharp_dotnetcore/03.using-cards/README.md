This sample demonstrates using [Adaptive dialog][1] and [Language Generation][2] PREVIEW features to achieve the same functionality that the waterfall based cards sample [here][3].

This sample uses preview packages available on the [BotBuilder MyGet feed][4].

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/csharp_dotnetcore/03.using-cards`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `experimental/adaptive-dialog/csharp_dotnetcore/03.using-cards` folder
  - Select `CardsBot.csproj` file
  - Press `F5` to run the project


## To debug adaptive dialogs
- You can install and use [this visual studio code extension][extension] to debug Adaptive dialogs. 

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction)
- [Bot basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/azure/bot-service/bot-concepts)
- [Activity processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing)
- [Rich cards](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-add-media-attachments?view=azure-bot-service-4.0&tabs=csharp#send-a-hero-card)

[41]: ../../README.md
[1]:../README.md
[2]:../language-generation/README.md
[3]:../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger