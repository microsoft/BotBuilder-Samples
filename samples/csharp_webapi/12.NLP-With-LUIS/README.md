﻿This sample shows how to integrate LUIS to a bot with ASP.Net Web API and Application Insights. 

## To try this sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-dotnet.git
```
## Prerequisites
### Set up Luis
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Click on the `Import new app` button.
- Click on the `Choose File` and select [LUIS-Reminders.json](LUIS-Reminders.json) from the `BotBuilder-Samples\samples\csharp_webapi\12.NLP-With-LUIS\CognitiveModels` folder.
- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY&verbose=true&timezoneOffset=0&q= 

    - The `AppID` can be found in "Application Information"
    - The `SubscriptionKey` can be found in "Keys and Endpoints", under the `Key 1` column
    - The `region` can be found in "Keys and Endpoints", under the `Region` column
    - The Version is listed on the page.

- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your Authoring Key.
    You can find this under your user settings at [luis.ai](https://www.luis.ai). Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
- **Important:** Ensure that `LuisKey` in `LuisBot.cs` matches the `name` property of your LUIS endpoint in your `nlp-with-luis.bot` file.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
### (Optional) Install LuDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown) to help describe language understanding components for your bot.
### Install Application Insights
  -  Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net-core) to set up your Application Insights service.
  - Note: The Application Insights will automatically update the [appsettings.json](appsettings.json) file.
### Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\samples\csharp_webapi\12.NLP-With-LUIS`) and open `LuisBot.csproj` in Visual studio 
- Run the project (press `F5` key). 
### Visual studio code
- Open `BotBuilder-Samples\samples\csharp_webapi\12.NLP-With-LUIS` sample folder
- Bring up a terminal, navigate to `BotBuilder-Samples\samples\csharp_webapi\12.NLP-With-LUIS` folder.
- Type `dotnet run`.
## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).
### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\samples\csharp_webapi\12.NLP-With-LUIS` folder
- Select `nlp-with-luis.bot` file
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Luis documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)