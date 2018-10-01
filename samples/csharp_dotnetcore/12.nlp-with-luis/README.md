﻿This sample shows how to integrate LUIS to a bot with ASP.Net Core 2. 

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
## Prerequisites
### Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Click on the `Import new app` button.
- Click on the `Choose File` and select [LUIS-Reminders.json](LUIS-Reminders.json) from the `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis/CognitiveModels` folder.
- Update [BotConfiguration.bot](BotConfiguration.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for
	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/{LuisAppID}?subscription-key={LuisSubscriptionKey}&verbose=true&timezoneOffset=0&q= 
    
    you will have something similar to this in the services section of your .bot file to run this sample:
    ```
    {
      "type": "luis",
      "name": "LuisBot", //This has to match the value for `LUIS_CONFIGURATION` in the index.js file
      "id": "",
      "appId": "{LuisAppID}",
      "authoringKey": "", //{LuisAuthoringKey} or
      "subscriptionKey": "{LuisSubscriptionKey}"
      "version": "0.1",
      "region": "westus"
    },
    ```

    The Version is listed on the page.
    Note: Enter the either "authoringKey" OR "subscriptionKey", not both
- Update [BotConfiguration.bot](BotConfiguration.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
### (Optional) Install LUDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUDown) to help describe language understanding components for your bot.

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis`) and open `LuisBot.csproj` in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` sample folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframeworkemulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` folder
- Select BotConfiguration.bot file
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
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

