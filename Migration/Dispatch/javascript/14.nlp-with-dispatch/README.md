# NLP with Dispatch

Bot Framework v4 NLP with Dispatch bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that relies on multiple [LUIS.ai](https://www.luis.ai) and [QnAMaker.ai](https://www.qnamaker.ai) models for natural language processing (NLP).

Use the Dispatch model in cases when:

- Your bot consists of multiple language modules (LUIS + QnA) and you need assistance in routing user's utterances to these modules in order to integrate the different modules into your bot.
- Evaluate quality of intents classification of a single LUIS model.
- Create a text classification model from text files.

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses the Dispatch service to route utterances as it demonstrates the use of multiple LUIS models and QnA maker services to support multiper conversational scenarios.

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```

### Use Dispatch with Mulitple LUIS and QnA Models

To learn how to configure Dispatch with multiple LUIS models and QnA Maker services, refer to the steps found [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0).

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/14.nlp-with-dispatch`

    ```bash
    cd samples/javascript_nodejs/14.nlp-with-dispatch
    ```

- Install modules

    ```bash
    npm install
    ```

- Setup Dispatch

    The prerequisite outlined above contain the steps necessary to configure Dispatch with multiple LUIS models and QnA Maker services.  Refer to [Use multiple LUIS and QnA models](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0) for directions to setup and configure Dispatch.

- Start the bot

    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Interacting with the bot

Once you are comfortable with the concepts presented in this sample, you may want to configure Dispatch using a CLI tool.  [Dispatch CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and/ or QnA Maker Knowledge Bases you have for your bot.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
