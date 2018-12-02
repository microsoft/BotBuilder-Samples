# welcome-users
Bot Framework v4 welcome users bot sample

This sample creates a bot that welcomes user when they join the conversation. The welcoming pattern shown in this bot is applicable for personal one-to-one conversation with bots.

## Prerequisites
- [Node.js][4] version 8.5 or higher

```bash
# determine node version
node --version
```

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/03.welcome-users`
    ```bash
    cd samples/javascript_nodejs/03.welcome-users
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
- File -> Open Bot Configuration and navigate to `samples/javascript_nodejs/03.welcome-users` folder
- Select `welcome-users.bot` file

# ConversationUpdate Activity Type
The [ConversationUpdate](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-activity-spec?view=azure-bot-service-3.0#conversation-update-activity) Activity describes a change in conversation members, for example when a new user (and/or) a bot joins the conversation. The channel sends this activity when a user (and/or) bot joins the conversation. It is recommended that you test your bot behavior on the target channel.

Bots that are added directly by a user, are mostly personal (1:1) conversation bots. It is a best practice to send a welcome message to introduce the bot tell a bit about its functionality. To do this, ensure that your bot responds to the `ConversationUpdate` message. Use the `membersAdded` field to identify the list of channel participants (bots or users) that were added to the conversation.

Your bot may proactively send a welcome message to a personal chat the first time a user initiates a personal chat with your bot. Use `UserState` to persist a flag indicating first user interaction with a bot.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Write directly to storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=jsechoproperty%2Ccsetagoverwrite%2Ccsetag)
- [Managing conversation and user state](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=js)

