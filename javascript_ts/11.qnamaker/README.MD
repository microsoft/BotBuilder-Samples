This sample shows how to create a bot that uses QnA Maker with Application Insights to get telemetry around bot and QnA Maker performance. This bot example uses [`applicationinsights`](https://www.npmjs.com/package/applicationinsights), [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://npmjs.com/package/dotenv). 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-js.git
    ```
- In a terminal, navigate to javascript_ts\11.qnamaker
    ```bash
    cd javascript_ts\11.qnamaker
    ```
- Point to the myget feed 
    ```bash
    npm config set registry https://botbuilder.myget.org/F/botbuilder-v4-js-daily/npm/
    ```
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to javascript_ts\11.qnamaker folder
- Select qnamaker.bot file

## Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a QnA Maker service
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/migrate-knowledge-base) to import the [smartLightFAQ.tsv](smartLightFAQ.tsv) to your newly created QnA Maker service
- Update [qnamaker.bot](qnamaker.bot) with your QnAMaker-Host, QnAMaker-KnowledgeBaseId and QnAMaker-EndpointKey. You can find this information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://qnamaker.ai)
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the Qna Maker CLI to deploy the model.

# QnA Maker service
QnA Maker enables you to power a question and answer service from your semi-structured content. 

One of the basic requirements in writing your own Bot service is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc. With QnA Maker, users can query your application in a natural, conversational manner. QnA Maker uses machine learning to extract relevant question-answer pairs from your content. It also uses powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)