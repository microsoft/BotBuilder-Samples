This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Core 2 and Application Insights. 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question
and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

## Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/migrate-knowledge-base) to
import the [sample.qna](sample.qna) to your newly created QnA Maker service.
- Update [BotConfiguration.bot](BotConfiguration.bot) with your kbid (KnowledgeBase Id) and endpointKey in the "qna" services section. You can find this
information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai).
- Name of your QnA Bot should be "QnABot".
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net-core) to set up your Application Insights service.
  - Note: The Application Insights will automatically update the [appsettings.json](appsettings.json) file.
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the
Qna Maker CLI to deploy the model.


## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights`) and open QnABot.csproj in Visual Studio.
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` sample folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` folder.
- Select the BotConfiguration.bot file.

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

- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)