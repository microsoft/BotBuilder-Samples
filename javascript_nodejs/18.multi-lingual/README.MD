# Multilingual Bot Sample

## Concepts introduced in this sample

Translation Middleware: We create a translation middleware than can translate text from bot to user and from user to bot, allowing the creation of multilingual bots. 
Users can specify their language preference, which is stored in the user state. The translation middleware translates to and from the user's preferred language.

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call. 
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

 This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs\18.multi-lingual
    ```bash
    cd javascript_nodejs\18.multi-lingual
    ```
- Point to the MyGet feed 
    ```bash
    npm config set registry https://botbuilder.myget.org/F/botbuilder-v4-js-daily/npm/
    ```
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
    Alternatively you can also use nodemon via
    ```bash
    npm i & npm run watch
    ```
- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
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
- File -> Open Bot Configuration and navigate to javascript_nodejs\18.multi-lingual folder
- Select multilingual.bot file

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)