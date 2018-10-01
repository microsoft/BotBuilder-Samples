# Concepts introduced in this sample

Translation Middleware: We create a translation middleware than can translate text from bot to user and from user to bot, allowing the creation of multi-lingual bots. 
The middleware is driven by user state. This means that users can specify their language preference, and the middleware automatically will intercept messages back and forth and present them to the user in their preferred language.
Users can change their language preference anytime, and since this gets written to the user state, the middleware will read this state and instantly modify its behavior to honor the newly selected preferred langugage.

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call. 
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Overview

In this sample, we create a simple bot that prompts user for their preferred language, and stores the user's preferred language selection in the user state. 
We also create a middleware that reads user preferred language, and if it is different from the default language (English), calls the Microsoft Translator Text API to translate to and from the user's preferred langugage.
This means that the bot always receives utterances in English, while users writes and gets responses in their selected language.

Note that this is a very simple example, but shows very powerful principles. 
The translation middleware allows us to intercept and traslate messages, and the user preferences stored in the user state at the application level allows us to influence the middleware behavior.
A more ellaborated next step would be to use the Microsoft Translator Text API to detect language, and if the user changes language, automatically switch to that language, without explicitly prompting but running detection on every step.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/17.multilingual-bot` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites

## Microsoft Translator Text API

To consume the Microsoft Translator Text API, first obtain a key following the instructions in the [Microsoft Translator Text API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup). 
Paste the key in the ```translationKey``` placeholder within the appsettings.json file.


## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/17.multilingual-bot`) and open MessageRoutingBot.csproj in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/17.multilingual-bot` sample folder.
- Bring up a terminal, navigate to botbuilder-samples/samples/csharp_dotnetcore/17.multilingual-bot folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/17.multilingual-bot` folder
- Select MessageRouting.bot file

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
- [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/)