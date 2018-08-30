This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Core 2 with Application Insights. 

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

## Visual studio
- Navigate to the samples folder (BotBuilder-Samples\11.QnAMaker) and open AspNetCore-QnA-Bot.csproj in Visual studio 
- Hit F5

## Visual studio code
- Open BotBuilder-Samples\11.QnAMaker sample folder.
- Bring up a terminal, navigate to BotBuilder-Samples\11.QnAMaker folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to BotBuilder-Samples\11.QnAMaker folder
- Select AspNetCore-QnA-Bot.bot file


# QnA Maker service
QnA Maker enables you to power a question and answer service from your semi-structured content. 

One of the basic requirements in writing your own Bot service is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc. With QnA Maker, users can query your application in a natural, conversational manner. QnA Maker uses machine learning to extract relevant question-answer pairs from your content. It also uses powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)