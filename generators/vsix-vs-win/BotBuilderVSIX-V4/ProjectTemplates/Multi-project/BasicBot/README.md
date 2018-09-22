# basic-bot
This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com),
- - Use [LUIS](https://luis.ai) to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user
# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template` with your botFileSecret.  
  - For Azure Bot Service bots, you can find the botFileSecret under application settings.
  - If you use [MSBot CLI](https://github.com/microsoft/botbuilder-tools) to encrypt your bot file, the botFileSecret will be written out to the console window.
  - If you used [Bot Framework Emulator **V4**](https://github.com/microsoft/botframework-emulator) to encrypt your bot file, the secret key will be available in bot settings. 

## Prerequisites
### Set up LUIS via Command Line
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on your name in the upper right hand corner and the `Settings` drop down menu.
- Note your `Authoring Key` as you will need this later.
- In a command line  session:
Navigate to sample:
`cd BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template`
Install LUIS command line tool:
`npm install -g luis-apis`
Create a new LUIS application:
`luis import application --in .\CognitiveModels\basic-bot.luis --authoringKey "<Your Authoring Key>" 
--endpointBasePath https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY`  
### Set up LUIS via Portal
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on the `Choose File` and select [basic-bot.luis](basic-bot.luis) from the `BotBuilder-Samples\csharp_dotnetcore\13.Basic-Bot-Template\CognitiveModels` folder.
- Update [BasicBot.bot](BasicBot.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for
	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY&verbose=true&timezoneOffset=0&q= 
    - AppId = XXXXXXXXXXXXX
    - SubscriptionKey = YYYYYYYYYYYY
    - Region =  westus
    The Version is listed on the page.
- Update [BotConfiguration.bot](BotConfiguration.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
- Update [BotConfiguration.bot](BotConfiguration.bot) file to ensure the `Id` property on the `luis` service type is set to `basic-bot-LUIS`.
## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template`) and open `BasicBot.csproj` in Visual Studio.
- Press F5.
## Visual Studio Code
- Open `BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template` folder.
- Type 'dotnet run'.
## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).
### Connect to bot using Bot Framework Emulator
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\samples\csharp_dotnetcore\13.Basic-Bot-Template` folder
- Select `BotConfiguration.bot` file

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 
To install all Bot Builder tools - 
```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
# Further reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [LUIS](https://luis.ai)
- [Prompt Types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [QnA Maker](https://qnamaker.ai)

