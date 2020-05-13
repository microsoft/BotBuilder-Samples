# CoreBot

This sample demonstrates using [Adaptive dialog][51] and [Language Generation][52] features to achieve the same functionality that the waterfall based core bot sample [here][53].

This bot has been created using [Bot Framework][1], it shows how to:
- Use **[Language Generation][41]** to power bot's responses
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

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

- In a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/03.core-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/adaptive-dialog/03.core-bot` folder
  - Select `CoreBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## LUIS Setup
### Using LUIS portal
- Navigate and sign in to [luis.ai][11]
- Under "My apps", click on "Import new app"
- Click on "Choose app file (JSON format) ..."
- Select `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/04.core-bot/Dialogs/RootDialog.lu.json
- Once the application is imported
    - Click on 'Train' to train the application
    - Click on 'Publish' to publish the application.
- Update appsettings.json
    - You can get your 'Authoring key' by following instructions [here][9]
    - You can get your application id and endpoint region by following instructions [here][10]

### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g @microsoft/botframework-cli
```
- Get your [LUIS authoring key](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-concept-keys)
- In a command prompt, navigate to `samples/csharp_dotnetcore/adaptive-dialog/03.core-bot`\
- To create/ train and publish a LUIS application (note you can re-run this command after you make any updates to .lu files)
```bash
> bf luis:build --in . --out generated --log --botName CoreBot --authoringKey <your-luis-authoring-key>
```
- Copy the Application Id to appsettings.json. This information is available under generated\luis.settings.\<alias>.\<region>.json.
- Add your endpoint key as well as endpoint name to appsettings.json.
  - Unless you assign a subscription key to your LUIS application, endpoint key is the same as the authoring key.
  - endpoint is usually https://\<region>.api.cognitive.microsoft.com. e.g. https://westus.api.cognitive.microsoft.com

# Further reading
- [Adaptive dialogs](https://aka.ms/adaptive-dialogs)
- [Language generation](https://aka.ms/language-generation)
- [Adaptive Expressions](https://aka.ms/adaptive-expressions)
- [.lu file format](https://aka.ms/lu-file-format)
- [.lg file format](https://aka.ms/lg-file-format)
- [.qna file format](https://aka.ms/qna-file-format)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction)
- [Bot basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/azure/bot-service/bot-concepts)
- [Activity processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing)

[1]: https://dev.botframework.com
[2]: https://nodejs.org/en/download/
[3]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[4]: https://dotnet.microsoft.com/download
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[9]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings
[10]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-regions
[11]: https://luis.ai
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

[51]:https://aka.ms/adaptive-dialogs
[52]:https://aka.ms/language-generation
[53]:../13.core-bot