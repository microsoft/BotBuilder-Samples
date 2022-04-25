# NLP with Orchestrator

Bot Framework v4 NLP with Orchestrator (PREVIEW) bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that relies on multiple [LUIS.ai](https://www.luis.ai) and [Custom Question Answering in Language Service][LS] models for natural language processing (NLP).

Use the Orchestrator dispatch model in cases when:

- Your bot consists of multiple language modules (LUIS + CustomQA) and you need assistance in routing user's utterances to these modules in order to integrate the different modules into your bot.
- Create a text classification model from text files.

## Overview

This bot uses Orchestrator to route user utterances to multiple LUIS models and QnA maker services to support multiple conversational scenarios.

## Prerequisites

| OS      | Version    | Architectures   |
| ------- | ---------- | --------------- |
| Windows | 10 (1607+) | ia32 (x86), x64 |
| MacOS | 10.15+ | x64 |
| Linux | Ubuntu 18.04, 20.04 | x64|



This sample **requires** prerequisites in order to run.

- Install latest supported version of [Visual C++ Redistributable](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)

- Install latest [Bot Framework Emulator](https://github.com/microsoft/BotFramework-Emulator/releases)

- [Node.js](https://nodejs.org) version 10.14 or higher
    ```bash
    > node --version
    ```
    
- Install BF CLI with Orchestrator plugin
    ```bash
    > npm i -g @microsoft/botframework-cli
    ```
    Make sure bf orchestrator command is working and shows all available orchestrator commands
    ```bash
    > bf orchestrator
    ```
    
## To try this bot sample

- Clone the repository
    ```bash
    > git clone https://github.com/microsoft/botbuilder-samples.git
    ```
    
- CD samples\javascript_nodejs\14.nlp-with-orchestrator
    ```bash
    > cd samples\javascript_nodejs\14.nlp-with-orchestrator
    ```
    
- Configure the LUIS applications (HomeAutomation and Weather) required for this sample.
    - Get your [LUIS authoring key](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-concept-keys)
    ```bash
    > bf luis:build --in CognitiveModels --authoringKey <YOUR-KEY> --botName <YOUR-BOT-NAME>
    ```
    - Update application settings in `./.env`
    
- Configure your CustomQA Project
  - Configure knowledge base of the project
    - Follow instructions [here][Quickstart] to create a Custom question answering project. You will need this project's name to be used as `ProjectName` in [.env](.env) file.
    - Visit [Language Studio][LS] and open created project.
    - Go to `Edit knowledge base` -> Click on `...` -> Click on `Import questions and answers` -> Click on `Import as TSV`.
    - Import [CustomQA.tsv](cognitiveModels/CustomQA.tsv) file.
    - You can test your knowledge base by clicking on `Test` option.
    - Go to `Deploy knowledge base` and click on `Deploy`.

  - Follow these steps to update [.env](.env) file.
    - In the [Azure Portal][Azure], go to your resource.
    - Go to `Keys and Endpoint` under Resource Management.
    - Copy one of the keys as value of `LanguageEndpointKey` and Endpoint as value of `LanguageEndpointHostName` in [.env](.env) file.
    - `ProjectName` is the name of the project created in [Language Studio][LS].
    
- Configure Orchestrator to route utterances to LUIS/QnA language services set up above
    - Download Orchestrator base model
    ```bash
    > mkdir model
    > bf orchestrator:basemodel:get --out ./model
    ```
    - Create the Orchestrator snapshot
    ```bash
    > mkdir generated
    > bf orchestrator:create --hierarchical --in ./cognitiveModels --out ./generated --model ./model
    ```
    The *hierarchical* flag creates top level intents in the snapshot file derived from the .lu/.qna/.tsv file names in the input folder.   As a result,  the example utterances are mapped to *HomeAutomation*, *CustomQA* and *Weather* intents/labels.

    - Verify .env has the following:

      ```
      ModelFolder=./model
      SnapshotFile=./generated/orchestrator.blu
      ```

- Install modules

    ```bash
    > npm install
    ```
    
- Start the bot

    ```bash
    > npm start
    ```

## Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [BF Orchestrator Command Usage](https://github.com/microsoft/botframework-sdk/blob/main/Orchestrator/docs/BFOrchestratorUsage.md)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[Azure]: https://portal.azure.com/
[LS]: https://language.cognitive.azure.com/
