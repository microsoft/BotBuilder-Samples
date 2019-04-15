# Luis Translator Bot Sample

This sample shows how to translate incoming and outgoing text using a custom middleware and the [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/).

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)


## Overview

In this sample, we create a simple hotel reservation bot that uses LUIS and Microsoft Translator to provide multilingual abilities to bots without the need to train different models for LUIS,
the sample intoduces a wrapper around the raw translation API that can provide some customizations to the sample like :
- no-translate-list of patterns that the user doesn't want to translate
- a custom configured language vocab dictionary that is used to override the raw translation for some vocab with a predfined user translations.
We also use two middlewares to intercept and handle the requests for the bot, LuisRecognizerMiddleware and TranslationMiddleware

## To try this sample
- Clone the repository
```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
```
### [Required] Getting set up with LUIS.ai model
- Navigate to [LUIS Portal](http://luis.ai)
- Click the `Sign in` button
- Click on `My apps` button
- Click on `Import new app`
- Click on the `Choose File` and select [HotelReservation.json](HotelReservation.json) from the `botbuilder-samples\experimental\csharp_dotnetcore\multilingual-luis-bot\Dialogs\HotelReservation\` folder.
- Update [BotConfiguration.bot](BotConfiguration.bot) file with your AppId, SubscriptionKey, Region and Version.
    You can find this information under "Manage" tab for your LUIS application at [LUIS portal](https://www.luis.ai).
    - The `AppID` can be found in "Application Information"
    - The `SubscriptionKey` can be found in "Keys and Endpoints", under the `Key 1` column
    - The `region` can be found in "Keys and Endpoints", under the `Region` column
### [Required] Getting set up with translator service
-To consume the Microsoft Translator Text API, first obtain a key following the instructions in the [Microsoft Translator Text API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup).
Paste the key in the ```translationKey``` placeholder within the appsettings.json file.

### (Optional) edit in customizable no-translate-patterns and language dictionary to give your bot translations more customization according to your scenario
- File -> Open bot and navigate to `botbuilder-samples\experimental\csharp_dotnetcore\multilingual-luis-bot\Dialogs\HotelReservation\` folder
- Modify in patterns.json and/or dictionary.json to enrich your customizations.

### (Optional) Install LUDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUDown) to help describe language understanding components for your bot.

### (Optional) Install Chatdown
- (Optional) Install the Chatdown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown) to generate a .transcript file from a .chat file to generate mock transcripts.

### Visual studio
- Navigate to the `botbuilder-samples\experimental\csharp_dotnetcore\multilingual-luis-bot\` and open MultilingualLuisBot.csproj in visual studio
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples\experimental\csharp_dotnetcore\multilingual-luis-bot\` sample folder.
- Bring up a terminal, navigate to multilingual-luis-bot folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples\experimental\csharp_dotnetcore\multilingual-luis-bot\` folder
- Select [BotConfiguration.bot](BotConfiguration.bot) file

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

# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

# Bot Translator
Bot translator allows your application to support multiple languages without an explicit need to train different language understanding models for each language.

# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
- [Bot Translator](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-translation?view=azure-bot-service-4.0&tabs=cs)
- [LUDown](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown)
- [Chatdown](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown)
