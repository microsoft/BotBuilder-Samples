# NLP with Orchestrator

Bot Framework v4 NLP with Orchestrator (PREVIEW) bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that relies on multiple [LUIS.ai](https://www.luis.ai) and [QnAMaker.ai](https://www.qnamaker.ai) models for natural language processing (NLP).

Use the Orchestrator Dispatch model in cases when:

- Your bot consists of multiple language modules (LUIS + QnA) and you need assistance in routing user's utterances to these modules in order to integrate the different modules into your bot.
- Create a text classification model from text files.

## Overview

This bot uses Orchestrator to route user utterances to multiple LUIS models and QnA maker services to support multiple conversational scenarios.

## Prerequisites

This sample **requires** prerequisites in order to run.
- Install latest supported version of [Visual C++](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)
- Install latest [Bot Framework Emulator](https://github.com/microsoft/BotFramework-Emulator/releases)
- [Node.js](https://nodejs.org) version 10.14 or higher
    ```bash
    > node --version
    ```
- Install BF CLI with Orchestrator plugin
    - Install bf cli 
    ```bash
    > npm i -g @microsoft/botframework-cli
    ```
    - Install bf orchestrator
    ```bash
    > bf plugins:install @microsoft/bf-orchestrator-cli@beta
    ```
      To reinstall bf orchestrator plugin, uninstall previous version and then run the install command again.
      
      Uninstall command:
    ```bash
    > bf plugins:uninstall @microsoft/bf-orchestrator-cli
    ```
    - Make sure bf orchestrator command is working and shows all available orchestrator commands
    ```bash
    > bf orchestrator
    ```
    
## To try this bot sample

- Clone the repository
    ```bash
    > git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- CD experimental/orchestrator/javascript_nodejs/01.dispatch-bot
    ```bash
    > cd experimental/orchestrator/javascript_nodejs/01.dispatch-bot
    ```
- Configure the LUIS application required for this sample.
    - Get your [LUIS authoring key](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-concept-keys)
    ```bash
    > bf luis:build --in CognitiveModels --authoringKey <YOUR-KEY>
    ```
    - Update application settings in `./appsettings.json`
- Configure the QnA Maker KB required for this sample.
    - Get your [QnA Maker Subscription key](https://docs.microsoft.com/en-us/azure/cognitive-services/QnAMaker/how-to/set-up-qnamaker-service-azure#create-a-new-qna-maker-service)
    ```bash
    > bf qnamaker:build --in CognitiveModels --subscriptionKey <YOUR-KEY>
    ```
    - Update kb information in `./appsettings.json`
- Configure Orchestrator: Download NLR model
    - You can view list of available models using this command.  Copy Version Id value from latest model and use it for --versionId parameter of the orchestrator:nlr:get command below.
    ```bash
    > bf orchestrator:nlr:list
    ```
    - Download the NLR model
    ```bash
    > mkdir model
    > bf orchestrator:nlr:get --versionId <version id> --out ./model --verbose
    ```
    - Build the Orchestrator snapshot
    ```bash
    > mkdir generated
    > bf orchestrator:create --hierarchical --in ./CognitiveModels --out ./generated --model ./model
    ```
    - Update orchestrator modelPath and snapshotPath information in `./.env`
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
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

