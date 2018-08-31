This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Web API with Application Insights. 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.a) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```


### Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/migrate-knowledge-base) to import the [smartLightFAQ.tsv](smartLightFAQ.tsv) to your newly created QnA Maker service.
- Update [appsettings.json](appsettings.json) with your QnAMaker-Host, QnAMaker-KnowledgeBaseId and QnAMaker-EndpointKey. You can find this information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://qnamaker.ai)
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net-core) to set up your Application Insights service.
  - Note: The Application Insights will automatically update the [appsettings.json](appsettings.json) file.

### Packages
- Click on the package you need to install. This sample requires - 
  + `Microsoft.Bot.Builder`
  + `Microsoft.Bot.Builder.AI.QnA`
  + `Microsoft.Bot.Builder.Integration.AspNet.WebApi`
  + `Microsoft.Bot.Builder.Configuration`
  + `Microsoft.Bot.Builder.Configuration`
  + `Unity.AspNet.WebApi`
  + `WebActivatorEx`


## Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_webapi\11.QnAMaker`) and open `AspNetWebApi-QnA-Bot.csproj` in Visual studio 
- Hit F5

## Visual studio code
- Open `BotBuilder-Samples\csharp_webapi\11.QnAMaker` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\csharp_webapi\11.QnAMaker` folder
- Type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_webapi\11.QnAMaker` folder
- Select `AspNetWebApi-QnA-Bot.bot` file

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)

