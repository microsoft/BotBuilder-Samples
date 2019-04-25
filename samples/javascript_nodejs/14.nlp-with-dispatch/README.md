# NLP with Dispatch sample
Bot Framework v4 NLP with Dispatch bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to create a bot that relies on multiple [LUIS.ai][11] and [QnAMaker.ai][12] models for natural language processing (NLP).

Use the Dispatch model in cases when:
- Your bot consists of multiple language modules (LUIS + QnA) and you need assistance in routing user's utterances to these modules in order to integrate the different modules into your bot.
- Evaluate quality of intents classification of a single LUIS model.
- Create a text classification model from text files.


## Prerequisites
This sample **requires** prerequisites in order to run.

### Overview
This bot uses the Dispatch service to route utterances as it demonstrates the use of multiple LUIS models and QnA maker services to support multiper conversational scenarios.

- Node.js version 10.14.1 or higher.
    ```bash
    # determine node version
    node --version
    ```

### Use Dispatch with Mulitple LUIS and QnA Models
To learn how to configure Dispatch with multiple LUIS models and QnA Maker services, refer to the steps found [here][41].

# To try this sample
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

    The prerequisite outlined above contain the steps necessary to configure Dispatch with multiple LUIS models and QnA Maker services.  Refer to [Use multiple LUIS and QnA models][41] for directions to setup and configure Dispatch.

- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Dispatch
Once you are confortable with the concepts presented in this sample, you may want to configure Dispath using a CLI tool.  [Dispatch CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and/ or QnA Maker Knowledge Bases you have for your bot.


# Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Using LUIS for Language Understanding][23]
- [LUIS documentation][24]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[12]: https://www.qnamaker.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js
[24]: https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0
