# Facebook events
Bot Framework v4 facebook events bot sample

This sample shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies and optin events.
Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

## Prerequisites
- [Node.js][4] version 10.14 or higher
    ```bash
    # determine node version
    node --version
    ```
- Enable Facebook Channel
    The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy, and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-4.0).

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to s`amples/javascript_nodejs/23.facebook-events`
    ```bash
    cd samples/javascript_nodejs/23.facebook-events
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy this bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Facebook Quick Replies](https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/0)
- [Facebook PostBack](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/)
- [Facebook Opt-in](https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/)
