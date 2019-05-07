# Active learning in QnA Maker
This sample shows how to integrate Active Learning in a QnA Maker bot in NodeJS. Click [here][al#1] to know more about how to enable and use active learning .


## Concepts introduced in this sample
The [QnA Maker Service][19] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate how to use the Active Learning to generate suggestions for knowledge base.

## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `experimental/qnamaker-activelearning/javascript_nodejs`
    ```bash
    cd experimental/qnamaker-activelearning/javascript_nodejs
    ```
- Install modules
    ```bash
    npm install
    ```
- Setup QnA Maker Knowledgebase

    Assuming prerequisites have been installed:
    ```bash
    # log into Azure
    az login
    ```
    ```bash
    # set you Azure subscription
    az account set --subscription "<azure-subscription>"
    ```
    ```bash
    # Create QnA Maker service application
    msbot clone services --name "<your_bot_name>" --code-dir "." --location westus --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.2.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `experimental/qnamaker-activelearning/javascript_nodejs` folder
- Select `qnamaker.bot` file

# QnA Maker service
QnA Maker enables you to power a question and answer service from your semi-structured content.

One of the basic requirements in writing your own bot is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc. With QnA Maker, users can query your application in a natural, conversational manner. QnA Maker uses machine learning to extract relevant question-answer pairs from your content. It also uses powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

# Try Active Learning
- Once your QnA Maker service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample queries: "surface pro 4", "apps on your surface pro"

# Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.  To deploy your bot to Azure:

```bash
# login to Azure
az login
```

### Publishing Changes to Azure Bot Service
As you make changes to your locally running bot, you can deploy those changes to Azure Bot Service using a _publish_ helper.  See `publish.cmd` if you are on Windows or `./publish` if you are on a non-Windows platform.  The following is an example of publishing local changes to Azure:

```bash
# build the TypeScript bot before you publish
npm run build
```

```bash
# run the publish helper (non-Windows) to update Azure Bot Service.  Use publish.cmd if running on Windows
./publish
```

### Getting Additional Help with Deploying to Azure
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