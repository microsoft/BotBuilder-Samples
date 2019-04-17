# proactive messages sample
Bot Framework v4 proactive messages bot sample

This sample demonstrates how to send proactive messages to users by
capturing a conversation reference, then using it later to initialize
outbound messages.

# Concepts introduced in this sample
Typically, each message that a bot sends to the user directly relates to the user's prior input. In some cases,
a bot may need to send the user a message that is not directly related to the current topic of conversation. These
types of messages are called proactive messages.

Proactive messages can be useful in a variety of scenarios. If a bot sets a timer or reminder, it will need to
notify the user when the time arrives. Or, if a bot receives a notification from an external system, it may need
to communicate that information to the user immediately. For example, if the user has previously asked the bot to
monitor the price of a product, the bot can alert the user if the price of the product has dropped by 20%. Or,
if a bot requires some time to compile a response to the user's question, it may inform the user of the delay
and allow the conversation to continue in the meantime. When the bot finishes compiling the response to the
question, it will share that information with the user.

This project has a notify endpoint that will trigger the proactive messages to be sent to 
all users who have previously messaged the bot.

## Prerequisites
- [Node.js][4] version 10.14 or higher
    ```bash
    # determine node version
    node --version
    ```

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/16.proactive-messages`
    ```bash
    cd samples/javascript_nodejs/16.proactive-messages
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
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

Run your bot locally and open the emulator.

- Send a get request to `http://localhost:3978/api/notify` to proactively message users from the bot.
   ```bash
    curl -X get http://localhost:3978/api/notify
   ```


# Proactive Messages
In addition to responding to incoming messages, bots are frequently called on to send "proactive" messages
based on activity, scheduled tasks, or external events.

In order to send a proactive message using Botbuilder, the bot must first capture a conversation reference
from an incoming message using `TurnContext.getConversationReference()`. This reference can be stored for
later use.

To send proactive messages, acquire a conversation reference, then use `adapter.continueConversation()` to
create a TurnContext object that will allow the bot to deliver the new outgoing message.

# Deploy this bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Send proactive messages][23]
- [continueConversation Method][24]
- [getConversationReference Method][26]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0

[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0&tabs=js
[24]: https://docs.microsoft.com/en-us/javascript/api/botbuilder/botframeworkadapter#continueconversation
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-core/turncontext#getconversationreference
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md
