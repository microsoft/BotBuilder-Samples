# CoreBot With CLU

Bot Framework v4 core bot sample using the CLU Recognizer.

This bot has been created using [Bot Framework](https://dev.botframework.com); it shows how to:

- Use [CLU][CLU_ServiceDocHomepage] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses [Conversational Language Understanding (CLU)][CLU_ServiceDocHomepage], an AI based cognitive service, to implement language understanding. The service uses natively multilingual models, which means that users would be able to train their models in one language but predict in others. Users of the service have access to the [language studio][languagestudio], which simplifies the process of adding/importing data, labelling it, training a model, and then finally evaluating it. For more information, visit the official [service docs][CLU_ServiceDocHomepage].

This new version of [CoreBot - sample 13 -](https://github.com/microsoft/BotBuilder-Samples/tree/main/samples/csharp_dotnetcore/13.core-bot) illustrates that the new API covers the existing functionalities of the LUIS Recognizer and that you can easily migrate your existing Bot Framework code using this sample as a guide.

### Install .NET Core CLI

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

### Create a CLU Application to enable language understanding

The CLU model for this example can be found under `CognitiveModels/FlightBooking.json` and the CLU language model setup, training, and application configuration steps can be found [here][CLU_ServiceQuickStart].

Once you created the CLU model, update `appsettings.json` with your `CluProjectName`, `CluDeploymentName`, `CluAPIKey` and `CluAPIHostName`.

```json
  "CluProjectName": "Your CLU project name",
  "CluDeploymentName": "Your CLU model deployment name",
  "CluAPIKey": "Your CLU Subscription key here",
  "CluAPIHostName": "Your CLU Host Name (i.e: sampleLanguageResource.cognitiveservices.azure.com)"
```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/90.core-bot-with-clu/90.core-bot-with-clu`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/90.core-bot-with-clu/90.core-bot-with-clu` folder
  - Select `CoreBotWithCLU.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Gathering Input Using Prompts](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=csharp)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[CLU_ServiceDocHomepage]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/overview
[CLU_ServiceQuickStart]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/quickstart
[languagestudio]: https://language.azure.com/