This sample shows how to create a bot that uses QnA Maker with Application Insights to get telemetry around bot and QnA Maker performance. This bot example uses [`applicationinsights`](https://www.npmjs.com/package/applicationinsights), [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://npmjs.com/package/dotenv). 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to samples/javascript_nodejs/20.qna-with-appinsights
    ```bash
    cd samples/javascript_nodejs/20.qna-with-appinsights
    ```
- [Optional] Update the .env file under samples/javascript_nodejs/20.qna-with-appinsights with your botFileSecret
    For Azure Bot Service bots, you can find the botFileSecret under application settings.
- Install modules and start the bot
    ```bash
    npm i && npm start
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples/javascript_nodejs/20.qna-with-appinsights
- Select qna-with-appinsights.bot file

## Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a QnA Maker service
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-publish-knowledge-base#create-a-qna-maker-knowledge-base) to import the [smartLightFAQ.tsv](cognitiveModels/smartLightFAQ.tsv) to your newly created QnA Maker service
- Update [qna-with-appinsights.bot](qna-with-appinsights.bot) with your QnAMaker-Host, QnAMaker-KnowledgeBaseId and QnAMaker-EndpointKey. You can find this information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://qnamaker.ai)
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-nodejs) to set up your Application Insights Service.
    - Note: You will need to manually add the Application Insights Instrumentation Key to the qna-with-appinsights.bot file.
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the Qna Maker CLI to deploy the model.

# QnA Maker service
QnA Maker enables you to power a question and answer service from your semi-structured content. 

One of the basic requirements in writing your own Bot service is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc. With QnA Maker, users can query your application in a natural, conversational manner. QnA Maker uses machine learning to extract relevant question-answer pairs from your content. It also uses powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools - 

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)