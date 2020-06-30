# Todo bot with LUIS

This sample demonstrates using [Adaptive dialog][1],  [Language Generation][2] features with [LUIS][5] to demonstrate an end-to-end ToDo bot in action including support for interruptions.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Configure the necessary [LUIS applications](#LUIS-Setup) required to run this sample

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/06.todo-bot`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/adaptive-dialog/06.todo-bot` folder
  - Select `ToDoBotWithLUIS.csproj` file
  - Press `F5` to run the project


## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## LUIS Setup
### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g @microsoft/botframework-cli
```
- In a command prompt, navigate to `samples/csharp_dotnetcore/adaptive-dialog/06.todo-bot`
- In order for interruption to work effectively, you need to generate LU models that are cross-trained.
  - Cross training requires a definition of your dialog hierarchy. See Dialogs/DialogLuHierarchy.config.json as an example.
```bash
> cd Dialogs
> bf luis:cross-train --in . --out ../generated --config DialogLuHierarchy.config.json
```
- Get your [LUIS authoring key](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-concept-keys)
- To create, train and pubish LUIS applications for this bot
```bash
> bf luis:build --in ../generated --out ../generated --log --botName TodoBotWithLuis --authoringKey <Your LUIS Authoring key>
```
- This command writes out a bunch of .dialog files (which are useful if you are using declarative form of adaptive dialogs) as well as luis.settings.\<youralias>.\<region>.json file.
- Add the application IDs for the created applications from luis.settings.\<youralias>.\<region>.json to appsettings.json.
- Add endpoint and APIKey to appsettings.json.

# Further reading
- [Adaptive dialogs](https://aka.ms/adaptive-dialogs)
- [Language generation](https://aka.ms/language-generation)
- [Adaptive Expressions](https://aka.ms/adaptive-expressions)
- [.lu file format](https://aka.ms/lu-file-format)
- [.lg file format](https://aka.ms/lg-file-format)
- [.qna file format](https://aka.ms/qna-file-format)
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[1]:https://aka.ms/adaptive-dialogs
[2]:https://aka.ms/language-generation
[3]:../../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[5]:https://luis.ai
[6]:#LUIS-Setup
[7]:https://github.com/Microsoft/botbuilder-tools
[8]:https://nodejs.org/en/
[9]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings#authoring-key
[10]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys
[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger