# NLP with Dispatch sample
Bot Framework v4 NLP with Dispatch bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to create a bot that relies on multiple [LUIS.ai][11] and [QnAMaker.ai][12] models for natural language processing (NLP).

## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]


[Dispatch CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a tool to create and evaluate LUIS models used for NLP (Natural Language Processing). Dispatch works across multiple bot modules such as LUIS applications, QnA knowledge bases and other NLP sources (added to dispatch as a file type).

Use the Dispatch model in cases when:
- Your bot consists of multiple modules and you need assistance in routing user's utterances to these modules and evaluate the bot integration.
- Evaluate quality of intents classification of a single LUIS model.
- Create a text classification model from text files.

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
- Setup LUIS

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
    # Create the LUIS & QnA Maker service applications
    msbot clone services --name "<your_bot_name>" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location westus --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
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
- Navigate to `samples/javascript_nodejs/14.nlp-with-dispatch` folder
- Select `nlp-with-dispatch.bot` file

# Dispatch
[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and/ or QnA Maker Knowledge Bases you have for your bot. For this sample, you would have already created 2 LUIS applications (Home Automation and Weather) and one QnA Maker Knowledge base.



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
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Using LUIS for Language Understanding][23]
- [LUIS documentation][24]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [msbot CLI][9]
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
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
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
[41]: ./PREREQUISITES.md
