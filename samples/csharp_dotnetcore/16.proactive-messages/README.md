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
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/16.proactive-messages` with your botFileSecret. For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/16.proactive-messages`) and open `ProactiveBot.csproj` in Visual Studio.
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Bring up a console, navigate to `botbuilder-samples/samples/csharp_dotnetcore/16.proactive-messages` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).

Build run your bot locally and open two instances of the emulator.

1. In the first emulator, type "run" to simulate a job being added to the queue.
1. Copy the job number from the emulator log.
1. In the second emulator, type "done <jobNumber>", where "<jobNumber>" is the job number, without the angle brackets, that you copied in the previous step. This will cause the bot to complete the job.
1. Note that the bot sends a message proactively to the user in the first emulator when the job is completed.

### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/16.proactive-messages` folder
- Select `proactive-messages.bot` file
- Open two conversations in the emulator, see that the proactive message goes to the correct conversation

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
