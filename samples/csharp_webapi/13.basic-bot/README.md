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
- In a command line session, navigate to sample: 
```bash
cd BotBuilder-Samples/samples/csharp_webapi/13.Basic-Bot-Template
```

To configure any services this sample depends on. In order to install the needed and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md). 

Create LUIS json model file (this will consume two .lu files combined into one model):
```bash
> ludown parse toluis -l ./Dialogs -s -o ./CognitiveModels -n BasicBot
```
Create a new LUIS application:
```bash
> luis import application --in ./CognitiveModels/BasicBot.json --authoringKey "<Your Authoring Key>" --endpointBasePath "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY"
```
### Set up LUIS via Portal
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on the `Choose File` and select [basic-bot.luis](basic-bot.luis) from the `BotBuilder-Samples/csharp_webapi/13.Basic-Bot-Template/CognitiveModels` folder.
- Update [BasicBot.bot](BasicBot.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Manage" tab for your LUIS application at [LUIS portal](https://www.luis.ai). 
    - The `AppID` can be found in "Application Information" 
    - The `AuthoringKey` can be found in "Keys and Endpoints" 
    - The `SubscriptionKey` can be found in "Keys and Endpoints", under the `Key 1` column 
    - The `region` can be found in "Keys and Endpoints", under the `Region` column 
    The Version is listed on the page.
- Update [basic-bot.bot](basic-bot.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
- Update [basic-bot.bot](basic-bot.bot) file to ensure the name property is set to `BasicBot`.
## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples/samples/csharp_webapi/13.Basic-Bot-Template`) and open `BasicBot.csproj` in Visual Studio.
- Run the project (press `F5` key).
## Visual Studio Code
- Open `BotBuilder-Samples/samples/csharp_webapi/13.Basic-Bot-Template` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples/samples/csharp_webapi/13.Basic-Bot-Template` folder.
- Type `dotnet run`.
## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).
### Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples/samples/csharp_webapi/13.Basic-Bot-Template` folder
- Select `basic-bot.bot` file
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
