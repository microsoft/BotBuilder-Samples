# Multilingual Bot

Bot Framework v4 multilingual bot sample

This sample will present the user with a set of cards to pick their choice of language. The user can either change language by invoking the option cards, or by entering the language code (_en_/_es_). The bot will then acknowledge the selection.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to translate incoming and outgoing text using a custom middleware and the [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/).

## Concepts introduced in this sample

Translation Middleware: We create a translation middleware that can translate text from bot to user and from user to bot, allowing the creation of multi-lingual bots.

The middleware is driven by user state. This means that users can specify their language preference, and the middleware automatically will intercept messages back and forth and present them to the user in their preferred language.

Users can change their language preference anytime, and since this gets written to the user state, the middleware will read this state and instantly modify its behavior to honor the newly selected preferred language.

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Prerequisites

This sample **requires** prerequisites in order to run.

- [Node.js](https://nodejs.org) version 18 or higher

    ```bash
    # determine node version
    node --version
    ```

- [Microsoft Translator Text API key](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup)

    To consume the Microsoft Translator Text API, first obtain a key following the instructions in the [Microsoft Translator Text API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup).
    Paste the key in the `TranslatorKey` setting in the `.env` file, or use your preferred configuration and update the following line in `index.js` with your translation key:

    ```js
    const translator = new MicrosoftTranslator(process.env.translatorKey);
    adapter.use(new TranslatorMiddleware(translator, languagePreferenceProperty));
    ```


## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/17.multilingual-bot`

    ```bash
    cd samples/javascript_nodejs/17.multilingual-bot
    ```

- Install modules

    ```bash
    npm install
    ```

- Setup Translation API

    The reprequisites outlined above contain the steps necessary to use the Microsoft Translator Text API.  Refer to the above prerequisites if you have not already done so.

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

### Creating a custom middleware

Translation Middleware: We create a translation middleware than can translate text from bot to user and from user to bot, allowing the creation of multilingual bots.
Users can specify their language preference, which is stored in the user state. The translation middleware translates to and from the user's preferred language.

### Microsoft Translator Text API

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

### Add `translatorKey` to Application Settings

If you used the `.env` file to store your `translatorKey` then you'll need to add this key and its value to the Application Settings for your deployed bot.

- Log into the [Azure portal](https://portal.azure.com)
- In the left nav, click on `Bot Services`
- Click the `<your_bot_name>` Name to display the bot's Web App Settings
- Click the `Application Settings`
- Scroll to the `Application settings` section
- Click `+ Add new setting`
- Add the key `translatorKey` with a value of the Translator Text API `Authentication key` created from the steps above

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
