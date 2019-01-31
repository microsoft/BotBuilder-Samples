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

- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Manage" tab for your LUIS application at [LUIS portal](https://www.luis.ai).

    - The `AppID` can be found in "Application Information"
    - The `AuthoringKey` can be found in "Keys and Endpoints"
    - The `SubscriptionKey` can be found in "Keys and Endpoints", under the `Key 1` column
    - The `region` can be found in "Keys and Endpoints", under the `Region` column

    You will have something similar to this in the services section of your .bot file to run this sample:

    ```json
    {
        "type":"luis",
        "name":"<some name>",
        "appId":"<an app id>",
        "version":"<a version number>",
        "authoringKey":"<your authoring key>",
        "subscriptionKey":"<your subscription key>",
        "region":"<region>",
        "id":"<some number>"
    },
    ```

    The Version is listed on the page.
    Note: Enter the either `authoringKey` OR `subscriptionKey`, not both

- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
    NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
- **Important:** Ensure that `LuisKey` in `LuisBot.cs` matches the `name` property of your LUIS endpoint in your `nlp-with-luis.bot` file.
### (Optional) Install LUDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown) to help describe language understanding components for your bot.

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis`) and open `LuisBot.csproj` in Visual Studio 
- Run the project (press `F5` key)

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` sample folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframeworkemulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/12.nlp-with-luis` folder
- Select `nlp-with-luis.bot` file
# Deploy this bot to Azure

You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run

```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

