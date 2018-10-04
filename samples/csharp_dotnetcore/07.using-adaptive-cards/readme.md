This sample demonstrates the use of Adaptive Cards.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Adaptive Cards
Card authors describe their content as a simple JSON object. That content can then be rendered natively inside a host application,
automatically adapting to the look and feel of the host. For example, Contoso Bot can author an Adaptive Card through the Bot Framework,
and when delivered to Skype, it will look and feel like a Skype card. When that same payload is sent to Microsoft Teams, it will look
and feel like Microsoft Teams. As more host apps start to support Adaptive Cards, that same payload will automatically light up inside 
these applications, yet still feel entirely native to the app. Users win because everything feels familiar. Host apps win because they
control the user experience. Card authors win because their content gets broader reach without any additional work.
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards`) and open AdaptiveCardsBot.csproj in Visual Studio 
- Hit F5
## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards` folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/07.using-adaptive-cards`
- Type `dotnet run`.
## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
- In Visual Studio Code type `dotnet restore`
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

To install all Bot Builder tools - 

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Adaptive Cards](https://adaptivecards.io/)
