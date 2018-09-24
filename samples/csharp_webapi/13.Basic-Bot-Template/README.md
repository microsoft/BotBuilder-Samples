Basic bot template that puts together cards, NLP (LUIS) with ASP.Net Core 2. 
# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
## Prerequisites
### Set up LUIS via Command Line
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on your name in the upper right hand corner and the `Settings` drop down menu.
- Note your `Authoring Key` as you will need this later.
- In a command line  session:
Navigate to sample:
`cd BotBuilder-Samples\samples\csharp_webapi\13.Basic-Bot-Template`

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

Install LUDown
`npm i -g ludown'
Create LUIS json model file (this will consume two .lu files combined into one model):
`ludown parse toluis -l .\Dialogs -s -o .\CognitiveModels -n BasicBot`
Install LUIS command line tool:
`npm install -g luis-apis`
Create a new LUIS application:
`luis import application --in .\CognitiveModels\BasicBot.json --authoringKey "<Your Authoring Key>" 
--endpointBasePath https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY`  
### Set up LUIS via Portal
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on the `Choose File` and select [basic-bot.luis](basic-bot.luis) from the `BotBuilder-Samples\csharp_webapi\13.Basic-Bot-Template\CognitiveModels` folder.
- Update [BasicBot.bot](BasicBot.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for
	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY&verbose=true&timezoneOffset=0&q= 
    - AppId = XXXXXXXXXXXXX
    - SubscriptionKey = YYYYYYYYYYYY
    - Region =  westus
    The Version is listed on the page.
- Update [LuisBot.bot](LuisBot.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
- Update [LuisBot.bot](LuisBot.bot) file to ensure the name property is set to `BasicBot`.
## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\samples\csharp_webapi\13.Basic-Bot-Template`) and open `BasicBot.csproj` in Visual Studio.
- Press F5.
## Visual Studio Code
- Open `BotBuilder-Samples\samples\csharp_webapi\13.Basic-Bot-Template` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\samples\csharp_webapi\13.Basic-Bot-Template` folder.
- Type 'dotnet run'.
## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).
### Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\samples\csharp_webapi\13.Basic-Bot-Template` folder
- Select `BasicBot.bot` file
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
