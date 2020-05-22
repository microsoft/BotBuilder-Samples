# Integrating Composer dialogs

This sample demonstrates how to integrate dialogs built with [Bot Framework Composer](https://github.com/microsoft/botframework-composer)

It starts with [19.custom-dialogs][s1] sample and shows how to get adaptive dialogs and custom/ waterfall dialogs work together.

## Steps to add composer dialogs to a V4 bot
1. Update all packages to 4.8.x from Nuget
2. Add the following packages
    - `Microsoft.Bot.Builder.Dialogs.Declarative`
    - `Microsoft.Bot.Builder.Dialogs.Adaptive`
3. Update index.js and add resource explorer to find and load declarative assets.
4. Add and configure **DialogManager** in `dialogBot.js`. This internally takes care of saving state on each turn.
5. Update adapter to use storage, conversation state and user state.
6. Use BotFramework Composer to build a bot. 
    - Navigate to the folder on your local machine where your composer bot is. 
    - Locate and copy over contents of `composerBot` folder.
7. Add the composer dialog to your RootDialog.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```

-  [Bot framework composer][composer]    

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/javascript_nodejs/09.integrating-composer-dialogs`

    ```bash
    cd experimental/adaptive-dialog/javascript_nodejs/09.integrating-composer-dialogs
    ```

- Install modules

    ```bash
    npm install
    ```

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

## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Composer](https://github.com/microsoft/botframework-composer)
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Dialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/dialog)
- [Manage complex conversation flows with dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-dialog-manage-complex-conversation-flow?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)

[4]:https://www.npmjs.com/search?q=botbuilder

[s1]:../../19.custom-dialogs/README.md
[composer]:https://github.com/microsoft/botframework-composer
