# Multilingual Bot

Bot Framework v4 multilingual bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to translate incoming and outgoing text using a custom middleware and the [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/).

## Concepts introduced in this sample

Translation Middleware: We create a translation middleware that can translate text from bot to user and from user to bot, allowing the creation of multi-lingual bots.

The middleware is driven by user state. This means that users can specify their language preference, and the middleware automatically will intercept messages back and forth and present them to the user in their preferred language.

Users can change their language preference anytime, and since this gets written to the user state, the middleware will read this state and instantly modify its behavior to honor the newly selected preferred language.

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Prerequisites

- [.NET Core SDK][4] version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- [Microsoft Translator Text API key](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup)

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/17.multilingual-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/17.multilingual-bot` folder
  - Select `MultiLingualBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

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

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

### Add `translationKey` to Application Settings

If you used the `appsettings.json` file to store your `TranslationKey` then you'll need to add this key and its value to the Application Settings for your deployed bot.

- Log into the [Azure portal][10]
- In the left nav, click on `Bot Services`
- Click the `<your_bot_name>` Name to display the bot's Web App Settings
- Click the `Application Settings`
- Scroll to the `Application settings` section
- Click `+ Add new setting`
- Add the key `TranslationKey` with a value of the Translator Text API `Authentication key` created from the steps above

## Further reading

- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Bot State][23]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [.NET Core CLI tools][43]
- [Azure CLI][7]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]

[1]: https://dev.botframework.com
[4]: https://dotnet.microsoft.com/download
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[43]: https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x
