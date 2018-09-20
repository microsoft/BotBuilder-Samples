This sample demonstrates a simple echo bot with state with ASP.Net Core 2. The bot maintains a simple counter that increases with each message from the user.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples\samples\csharp_dotnetcore\02.echo-with-counter` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples\samples\csharp_dotnetcore\02.echo-with-counter`) and open EchoBotWithCounter.csproj in Visual Studio.
- Hit F5.

## Visual Studio Code
- Open `botbuilder-samples\samples\csharp_dotnetcore\02.echo-with-counter` sample folder.
- Bring up a terminal, navigate to `botbuilder-samples\samples\csharp_dotnetcore\02.echo-with-counter` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `botbuilder-samples\samples\csharp_dotnetcore\02.echo-with-counter` folder.
- Select `BotConfiguration.bot` file.
# Bot state
A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. Depending on what your bot is used for, you may even need to keep track of conversation state or store user related information for longer than the lifetime of one given conversation.

In this example, the bot's state is used to track number of messages.

 A bot's state is information it remembers in order to respond appropriately to incoming messages. The Bot Builder SDK provides classes for [storing and retrieving state data](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=js) as an object associated with a user or a conversation.

    - Conversation properties help your bot keep track of the current conversation the bot is having with the user. If your bot needs to complete a sequence of steps or switch between conversation topics, you can use conversation properties to manage steps in a sequence or track the current topic. Since conversation properties reflect the state of the current conversation, you typically clear them at the end of a session, when the bot receives an end of conversation activity.
    
    - User properties can be used for many purposes, such as determining where the user's prior conversation left off or simply greeting a returning user by name. If you store a user's preferences, you can use that information to customize the conversation the next time you chat. For example, you might alert the user to a news article about a topic that interests her, or alert a user when an appointment becomes available. You should clear them if the bot receives a delete user data activity.

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
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Write directly to storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=jsechoproperty%2Ccsetagoverwrite%2Ccsetag)
- [Managing conversation and user state](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=js)
