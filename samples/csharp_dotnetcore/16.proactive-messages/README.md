This sample demonstrates how to send proactive messages to users by
capturing a conversation reference, then using it later to initialize
outbound messages using ASP.Net Core 2.

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

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples\samples\csharp_dotnetcore\16.proactive-messages` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples\samples\csharp_dotnetcore\16.proactive-messages`) and open `ProactiveBot.csproj` in Visual Studio.
- Press F5.

## Visual Studio Code
- Open `botbuilder-samples\samples\csharp_dotnetcore\16.proactive-messages` sample folder.
- Bring up a console, navigate to `botbuilder-samples\samples\csharp_dotnetcore\16.proactive-messages` folder.
- type `dotnet run`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).

### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples\samples\csharp_dotnetcore\16.proactive-messages` folder
- Select `BotConfiguration.bot` file
- Open two conversations in the emulator, see that the proactive message goes to the correct conversation

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools - 

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
