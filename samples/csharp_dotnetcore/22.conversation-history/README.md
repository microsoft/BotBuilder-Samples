This sample demonstrates the use of SendConversationHistoryAsync API to upload conversation history stored in the conversation Transcript.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `conversation-history.bot` file under `botbuilder-samples/samples/csharp_dotnetcore/22.conversation-history` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/storage/common/storage-quickstart-create-account?tabs=portal) to create a Blob Storage service.
- Update [`conversation-history.bot`](conversation-history.bot) with your `connectionString` for Blob service. Also you need to update the `StorageConfigurationId` from [Startup.cs](Startup.cs) with the Blob service name.
> You can find your storage account's connection strings in the Azure portal. Navigate to SETTINGS > Access keys in your storage account's menu blade to see connection strings for both primary and secondary access keys.

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/22.conversation-history`) and open ConversationHistory.csproj in Visual Studio.
- Set the BLOB store connection-string in conversation-history.bot. Also you need to update the `StorageConfigurationId` from [Startup.cs](Startup.cs) with the Blob service name.
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/22.conversation-history` folder.
- Set the BLOB store connection-string in conversation-history.bot. Also you need to update the `StorageConfigurationId` from [Startup.cs](Startup.cs) with the Blob service name.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/22.conversation-history` folder.
- Select `ConversationHistory.bot` file.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)
