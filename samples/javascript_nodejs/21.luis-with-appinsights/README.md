This sample shows how to create a bot that uses Language Understanding (LUIS) with Application Insights to get telemetry around bot and your LUIS application's performance. This bot example uses [`applicationinsights`](https://www.npmjs.com/package/applicationinsights), [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://npmjs.com/package/dotenv). 

# Concepts introduced in this sample
[LUIS](https://www.luis.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to call LUIS to extract the intents from a user's message and use Application Insights to gather user-bot interaction telemetry.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to samples/javascript_nodejs/21.luis-with-appinsights
    ```bash
    cd samples/javascript_nodejs/21.luis-with-appinsights
    ```
- [Optional] Update the .env file under samples/javascript_nodejs/21.luis-with-appinsights with your botFileSecret
    For Azure Bot Service bots, you can find the botFileSecret under application settings.
- Install modules and start the bot
    ```bash
    npm i && npm start
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples/javascript_nodejs/21.luis-with-appinsights
- Select luis-with-appinsights.bot file

## Prerequisites
### Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Click on the `Import new app` button.
- Click on the `Choose File` and select [reminders.json](cognitiveModels/reminders.json) from the `botbuilder-samples/samples/javascript_nodejs/21.luis-with-appinsights/cognitiveModels` folder.
- Update [luis-with-appinsights.bot](luis-with-appinsights.bot) file with your AppId, SubscriptionKey, Region and Version. 
    You can find this information under "Publish" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for
	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/XXXXXXXXXXXXX?subscription-key=YYYYYYYYYYYY&verbose=true&timezoneOffset=0&q= 

    - AppId = XXXXXXXXXXXXX
    - SubscriptionKey = YYYYYYYYYYYY
    - Region =  westus

    The Version is listed on the page.
- Update [luis-with-appinsights.bot](luis-with-appinsights.bot) file with your Authoring Key.  
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.
NOTE: Once you publish your app on LUIS portal for the first time, it takes some time for the endpoint to become available, about 5 minutes of wait should be sufficient.
### (Optional) Install LUDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUDown) to help describe language understanding components for your bot.
### Install Application Insights
  -  Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-nodejs) to set up your Application Insights service.

# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools -

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)
- [LUDown](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)