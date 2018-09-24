This sample shows how to use suggested actions. This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to samples/javascript_nodejs/08.suggested-actions
    ```bash
    cd samples/javascript_nodejs/08.suggested-actions
    ```
- [Optional] Update the .env file under samples/javascript_nodejs/08.suggested-actions with your botFileSecret
    For Azure Bot Service bots, you can find the botFileSecret under application settings.
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
    Alternatively you can also use nodemon via
    ```bash
    npm i & npm run watch
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples/javascript_nodejs/08.suggested-actions folder
- Select suggested-actions.bot file

# Suggested Actions
Suggested actions enable your bot to present buttons that the user can tap to provide input. Suggested actions appear close to the composer and enhance user experience.
They enable the user to answer a question or make a selection with a simple tap of a button, rather than having to type a response with a keyboard.

Unlike buttons that appear within rich cards (which remain visible and accessible to the user even after being tapped), buttons that appear within the suggested actions pane will disappear after the user makes a selection. This prevents the user from tapping stale buttons within a conversation and simplifies bot development (since you will not need to account for that scenario).

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools - 

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Suggested Actions](https://docs.microsoft.com/en-us/azure/bot-service/nodejs/bot-builder-nodejs-send-suggested-actions?view=azure-bot-service-4.0)