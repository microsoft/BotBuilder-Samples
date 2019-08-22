# Active learning in QnA Maker
This sample shows how to integrate Active Learning in a QnA Maker bot in NodeJS. Click [here][al#1] to know more about how to enable and use active learning .


## Concepts introduced in this sample
The [QnA Maker Service][19] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate how to use the Active Learning to generate suggestions for knowledge base.

# Prerequisites

This samples **requires** prerequisites in order to run.

### Overview

This bot uses [QnA Maker Service](https://www.qnamaker.ai), an AI based cognitive service, to implement simple Question and Answer conversational patterns.

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```

### Create a QnAMaker Application to enable QnA Knowledge Bases

QnA knowledge base setup and application configuration steps can be found [here](https://aka.ms/qna-instructions).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/48.qnamaker-activelearning-bot`
    ```bash
    cd samples/javascript_nodejs/48.qnamaker-activelearning-bot
    ```
- Install modules
    ```bash
    npm install
    ```
- Setup QnAMaker

    The prerequisite outlined above contain the steps necessary to provision a QnA Knowledge Base on www.qnamaker.ai. QnA knowledge base setup and application configuration steps can be found [here](https://aka.ms/qna-instructions).

- Start the bot
    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# QnA Maker service
QnA Maker enables you to power a question and answer service from your semi-structured content.

One of the basic requirements in writing your own bot is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc. With QnA Maker, users can query your application in a natural, conversational manner. QnA Maker uses machine learning to extract relevant question-answer pairs from your content. It also uses powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

# Try Active Learning
- Once your QnA Maker service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample queries: "surface pro 4", "apps on your surface pro"

# Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

# Further reading
- [Active learning Documentation][al#1]
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [QnA Maker Documentation][23]
- [Activity Processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [msbot CLI][9]
- [QnA Maker CLI][24]
- [Azure Portal][10]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[19]: https://www.qnamaker.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview
[24]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md
[al#1]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/improve-knowledge-base