# QnAMaker Bot

Bot Framework v4 qnamaker bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to consume [QnAMaker.ai](https://qnamaker.ai) KBs within your [adaptive dialog][1] based bot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Install required [QnA Maker KB](#QnAMaker-Setup) required for this sample.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/07.qnamaker-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/adaptive-dialog/07.qnamaker-bot` folder
  - Select `QnAMaker.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## QnAMaker Setup
### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g @microsoft/botframework-cli
```
- In a command prompt, navigate to `samples/csharp_dotnetcore/adaptive-dialog/07.qnamaker-bot`
- Run qnamaker:build to create/ update, train and publish QnA Maker KBs required to run this bot. The content for the KB comes from .qna files under dialogs.
- Get your [QnA Maker subscription key](https://docs.microsoft.com/en-us/azure/cognitive-services/QnAMaker/how-to/set-up-qnamaker-service-azure#create-a-new-qna-maker-service)
```bash
> mkdir generated
> bf qnamaker:build --in Dialogs --out generated --log --botName QnAMakerSampleBot --subscriptionKey <your-key>
```
- This command writes out a bunch of .dialog files (which are useful if you are using declarative form of adaptive dialogs) as well as qnamaker.settings.\<youralias>.\<region>.json file. 
- Add the KB IDs for the created applications from qnamaker.settings.\<youralias>.\<region>.json to appsettings.json
```jsonc
// Example qnamaker.settings.<alias>.<region>.json file.
{
    "qna": {
        // copy these to appsettings.json
        "QnAMakerSample_en_us_qna": "",
        "hostname": ""
    }
}
```
- Get your EndpointKey using the following command
```bash
> bf qnamaker:endpointkeys:list --subscriptionKey <your-subscription-key>
```
- This command will write out your primary and secondary keys. Copy either key to `qna:endpointKey` in the appsettings.json file.

## Further reading

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
