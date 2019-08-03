# CoreBot
This sample demonstrates using [Adaptive dialog][51] and [Language Generation][52] PREVIEW features to achieve the same functionality that the waterfall based core bot sample [here][53].

This sample uses preview packages available on the [BotBuilder MyGet feed][54].

This bot has been created using [Bot Framework][1], it shows how to:
- Use **[Language Generation][41]** to power bot's responses
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Create a [LUIS application](#LUIS-Setup)

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/csharp_dotnetcore/04.core-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `experimental/adaptive-dialog/csharp_dotnetcore/04.core-bot` folder
  - Select `CoreBot.csproj` file
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

## LUIS Setup
### Using LUIS portal
- Navigate and sign in to [Luis.ai][11]
- Under "My apps", click on "Import new app"
- Click on "Choose app file (JSON format) ..."
- Select `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/04.core-bot/CognitiveModels/CoreBot.luis.json
- Once the application is imported
    - Click on 'Train' to train the application
    - Click on 'Publish' to publish the application.
- Update appsettings.json
    - You can get your 'Authoring key' by following instructions [here][9]
    - You can get your application id and endpoint region by following instructions [here][10]

### Using CLI
- Install [nodejs][2] version 8.5 or higher
- Install [botbuilder-tools][3] CLI
```bash
> npm i -g ludown luis-apis
```
- In a command prompt, navigate to `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/04.core-bot`
- To parse RootDialog.lu to a LUIS json model
```bash
> ludown parse toluis --in ./Dialogs/RootDialog.lu -o CognitiveModels -n CoreBot --out CoreBot.luis.json
```
- To create a new LUIS application using this model. Note: You see [here][9] for instructions on getting your authoirng key.
```bash
> luis import application --in ./CognitiveModels/CoreBot.luis.json --authoringKey <YOUR-AUTHORING-KEY>
```
- Copy the relevant Application Id, endpoint information as well as your authoring key to appsettings.json.
- To train and publish the LUIS application,
```bash
> luis train version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
> luis publish version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
```

[1]: https://dev.botframework.com
[2]: https://nodejs.org/en/download/
[3]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[4]: https://dotnet.microsoft.com/download
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[9]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings
[10]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-regions
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0
[24]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-waterfall?view=azure-bot-service-4.0
[27]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ../../README.md

[51]:../README.md
[52]:../language-generation/README.md
[53]:../../../samples/csharp_dotnetcore/13.core-bot
[54]:https://botbuilder.myget.org/gallery/botbuilder-declarative

[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger
