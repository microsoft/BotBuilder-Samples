This sample logs transcript activity events to Event Hub.  It creates an Application Insights custom telemetry processor, enqueues the transcript activities to an in-memory queue. The sample also constructs a background hosted service for ASP.NET Core, which dequeues transcript activities from the queue, batches activities until reaching batch size limit or configured maximum batch sending delay.  The background hosted service sends the batch when it reaches the max event batch size, or its oldest event has been in the batch for more than max batch delay seconds.

## To try this sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/27.eventhub-logger/EventHubLogger.csproj` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/27.eventhub-logger`) and open EventHubLogger.csproj in Visual Studio
- Run the project (press `F5` key).

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/27.eventhub-logger`
- Type `dotnet run`.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run

```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Testing the bot using Bot Framework Emulator V4
Microsoft Bot Framework Emulator is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
Launch Bot Framework Emulator
File -> Open Bot Configuration and navigate to botbuilder-samples/samples/csharp_dotnetcore/27.eventhub-logger folder
Input the bot URL.


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity Processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Event Hubs](https://docs.microsoft.com/en-us/azure/event-hubs/)
