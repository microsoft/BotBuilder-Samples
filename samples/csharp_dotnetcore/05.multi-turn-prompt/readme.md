This sample demonstrates a the use of multiple prompts with ASP.Net Core 2.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/05.multi-turn-prompt` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/05.multi-turn-prompt`) and open MultiTurnPromptsBot.csproj in Visual Studio.
- Hit F5.

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/05.multi-turn-prompt` sample folder.
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/05.multi-turn-prompt` folder.
- Type `dotnet run`.

## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Builder.Dialogs` and `Microsoft.Bot.Builder.Integration.AspNet.Core`.
- In Visual Studio Code type `dotnet restore`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/05.multi-turn-prompt` folder.
- Select `BotConfiguration.bot` file.
# Use Azure Blob Storage or Azure CosmosDB

##### In Visual Studio

Add the `Microsoft.Bot.Builder.Azure` Nuget package to your solution. That package is found at https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/.

For the Azure Blob Storage uncomment the following code from `Startup.cs`

```c#
const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
if (!(blobConfig is BlobStorageService blobStorageConfig))
{
	throw new InvalidOperationException($"The .bot file does not contain an blob storage with name '{StorageConfigurationId}'.");
}
// Default container name.
const string DefaultBotContainer = "<DEFAULT-CONTAINER>";
var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);

// Create Conversation State object.
// The Conversation State object is where we persist anything at the conversation-scope.
var userState = new UserState(dataStore);
options.State.Add(userState);
```

# Deploy this bot to Azure

You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher.

To install all Bot Builder tools.

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)
