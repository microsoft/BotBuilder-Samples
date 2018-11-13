# Facebook events sample

This sample shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies and optin events. 
Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to samples/javascript_nodejs/23.facebook-events
    ```bash
    cd samples/javascript_nodejs/23.facebook-events
    ```
- Install modules and start the bot
    ```bash
    npm i && npm start
    ```
    Alternatively you can also use nodemon via
    ```bash
    npm i && npm run watch
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples/javascript_nodejs/23.facebook-events folder
- Select facebook-events.bot file

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


## Enable Facebook Channel
The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy, and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-3.0).

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Facebook Quick Replies](https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/0)
- [Facebook PostBack](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/)
- [Facebook Optin](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/)