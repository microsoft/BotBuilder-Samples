# nlp with luis sample
Bot Framework v4 natural language processing with LUIS bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to use LUIS, a Natural Language Understanding service to implement language understanding in a bot. The bot will use LUIS to extract language intents from a user's message.

## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]

## To try this sample◊
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a console, navigate to `samples/javascript_typescript/12.nlp-with-luis`
    ```bash
    cd samples/javascript_typescript/12.nlp-with-luis
    ```
- Install modules
    ```bash
    npm install
    ```
- Build the bot source code
    ```bash
    npm run build
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
    # Create LUIS service application
    msbot clone services --name "<your_bot_name>" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location westus --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
    ```
- **Important:** Ensure that `LUIS_CONFIGURATION` in `index.ts` matches the `name` property of your LUIS endpoint in your `nlp-with-luis.bot` file.
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
- Navigate to `samples/javascript_typescript/12.nlp-with-luis` folder
- Select `nlp-with-luis.bot` file


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

## How to modify a language model
This sample uses a language model to train LUIS.  The source for the language model can be found in the `cognitiveModels\reminders.lu`.  The `.lu` (language understanding) file describes language understanding components for your bot.  `.lu` files are text files and can be modified to change what language your bot will understand.  The `ludown` CLI tool takes as input a `.lu` file and produces a `.json` file.  This `.json` file is then used as input to the `luis` CLI tool to train your LUIS application's language understanding model.

### Train and publish the LUIS models
If you modify `reminders.lu` you need to train and publish the LUIS model. You can do so using the `ludown` and `luis` CLI tools.

### Install `ludown` and `luis` CLI tools
```bash
# install the ludown CLI tool
npm install -g ludown
```
```bash
# install the LUIS CLI tool
npm install -g luis-apis
```
To learn more about the `ludown` CLI tool, refer to the documentation found [here](https://aka.ms/using-ludown).

To learn more about the `luis` CLI tool, refer to the documentation found [here](https://aka.ms/using-luis-cli).

### Example running `ludown` and `luis`
The following examples will train and publish a LUIS model:

```bash
msbot get "Reminders" | luis train version --wait --stdin
msbot get "Reminders" | luis publish version --stdin
```

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
