# NLP with LUIS
Bot Framework v4 NLP with LUIS bot sample

This sample shows how to create a bot that uses Language Understanding (LUIS).


# Concepts introduced in this sample
[Language Understanding (LUIS)](https://www.luis.ai) is a cloud-based API service that applies custom machine-learning intelligence to a user's conversational, natural language text to predict overall meaning, and pull out relevant, detailed information.

In this sample, we demonstrate how to call LUIS to extract the intents from a user's message.

# Prerequisites
- [Node.js][4] version 8.5 or higher
    ```bash
    # determine node version
    node --version
    ```
- [LUIS](https://www.luis.ai) service application
    - Navigate to [LUIS portal](https://www.luis.ai).
    - Click the `Sign in` button.
    - Click on `My Apps`.
    - Click on the `Import new app` button.
    - Click on the `Choose File` and select [reminders.json](cognitiveModels/reminders.json) from the `BotBuilder-Samples/javascript_nodejs/12.nlp-with-luis/cognitiveModels` folder.
    - Provide a name for the LUIS app
    - Click on the `Train` button. To train your language model.
    - Click on the `Publish` button.  To publish your trained language model.
    - Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your AppId, SubscriptionKey, Region and Version.
        You can find this information under "Keys and Endpoint" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for https://westus.settingsapi.cognitive.microsoft.com/luis/v2.0/apps/{LuisAppID}?subscription-key={LuisSubscriptionKey}&verbose=true&timezoneOffset=0&q=

        The Version is listed on the page.
        Note: Enter either the "authoringKey" OR "subscriptionKey", not both.
        [See an example .bot service configuration for using LUIS here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app).

    - Update [index.js](index.js) and set the `LUIS_CONFIGURATION` value to match the `name` field in your service declaration.

    - Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your Authoring Key.
        You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.

        NOTE: Once you publish your app on LUIS portal for the first time, it may take some time to go live.

    - Your .bot file should now include an item in the services array that looks like this:

    ```javascript
    {
        "type":"luis",
        "name":"<some name>",
        "appId":"<an app id>",
        "version":"<a version number>",
        "authoringKey":"<your authoring key>",
        "subscriptionKey":"<your subscription key>",
        "region":"<region>",
        "id":"<some number>"
    }
    ```

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a console, navigate to s`amples/javascript_nodejs/12.nlp-with-luis`
    ```bash
    cd samples/javascript_nodejs/12.nlp-with-luis
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_nodejs/12.nlp-with-luis` folder
- Select `nlp-with-luis.bot` file

# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

# Optional Command-line Tools for LUIS
- Install the `ludown` CLI tool [here](https://aka.ms/using-ludown) to help describe language understanding components for your bot.
- Install the `luis` CLI tool [here](https://aka.ms/using-luis-cli) to create and manage your LUIS applications.


# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on.

To install all Bot Builder tools -

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
