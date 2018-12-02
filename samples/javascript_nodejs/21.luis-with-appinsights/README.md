# luis with appinsights
Bot Framework v4 LUIS with AppInsights bot sample

This sample shows how to create a bot that uses Language Understanding (LUIS) with [Application Insights](https://www.npmjs.com/package/applicationinsights) to get telemetry around bot and your LUIS application's performance.

# Concepts introduced in this sample
[LUIS](https://www.luis.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to call LUIS to extract the intents from a user's message and use Application Insights to gather user-bot interaction telemetry.

## Prerequisites
- [Node.js][4] version 8.5 or higher
    ```bash
    # determine node version
    node --version
    ```
- Set up LUIS
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
- Setup Application Insights
    -  Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-nodejs) to set up your Application Insights service.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/21.luis-with-appinsights`
    ```bash
    cd samples/javascript_nodejs/21.luis-with-appinsights
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_nodejs/21.luis-with-appinsights`
- Select `luis-with-appinsights.bot` file

# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

# Optional Command-line Tool LUDown
- (Optional) Install the LUDown [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUDown) to help describe language understanding components for your bot.
    ```bash
    npm i -g ludown
    ```

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on.


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
