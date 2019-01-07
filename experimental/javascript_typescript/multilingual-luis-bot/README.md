# Luis Translator Bot Sample

This sample shows how to translate incoming and outgoing text using a custom middleware and the [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/). This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to experimental/javascript_typescript/multilingual-luis-bot
    ```bash
    cd experimental/javascript_typescript/multilingual-luis-bot
    ```
- [Optional] Update the .env file under experimental/javascript_typescript/multilingual-luis-bot with your botFileSecret
    For Azure Bot Service bots, you can find the botFileSecret under application settings.
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
    Alternatively you can also use nodemon via
    ```bash
    npm i & npm run watch
    ```
## Microsoft Translator Text API

To consume the Microsoft Translator Text API, first obtain a key following the instructions in the [Microsoft Translator Text API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup).
Paste the key in the `translationKey` setting in the .env file, or use your preferred configuration and update the following line in index.js with your translation key:

```js
adapter.use(new TranslatorMiddleware(languagePreferenceProperty, process.env.translatorKey));
```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `experimental/javascript_typescript/multilingual-luis-bot` folder
- Select luis-translator-bot.bot file

## Prerequisite
### Install TypeScript
In order to run this sample, you must have TypeScript installed.  To install TypeScript:
- Navigate to the [TypeScript portal](https://www.typescriptlang.org).
- Click the [Download](https://www.typescriptlang.org/#download-links) button.
- Follow the installation instructions for your development environment.

### Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Click on the `Import new app` button.
- Click on the `Choose File` and select [hoterReservation.json](cognitiveModels/hoterReservation.json) from the `BotBuilder-Samples/experimental/javascript_typescript/multilingual-luis-bot/cognitiveModels` folder.
- Update [luis-translator-bot.bot](luis-translator-bot.bot) file with your AppId, SubscriptionKey, Endpoint and Version.
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for
	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY&verbose=true&timezoneOffset=0&q=

    - AppId = XXXXXXXXXXXXX
    - SubscriptionKey = YYYYYYYYYYYY
    - Endpoint = https://westus.api.cognitive.microsoft.com/

    The Version is listed on the page. [See an example .bot service configuration for using LUIS here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app).
- Update [luis-translator-bot.bot](luis-translator-bot.bot) file with your Authoring Key.
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option. Add this to your .bot file's service configuration as `authoringKey`.

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
    "endpoint":"<region>",
    "id":"<some number>"
}
```

- Update [index.ts](src/index.ts) and set the `LUIS_CONFIGURATION` value to match the `name` or `id` field in your service declaration.

## Creating a custom middleware

Translation Middleware: We create a translation middleware than can translate text from from user to bot and apply some customizations like using a custon no-translate patterns or a custom vocab dictionary, allowing the creation of multilingual bots.

## LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

## Microsoft Translator Text API
The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

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
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [Translator documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/)
