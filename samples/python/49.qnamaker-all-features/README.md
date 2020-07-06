# QnA Maker

Bot Framework v4 QnA Maker bot sample. This sample shows how to integrate Multiturn and Active learning in a QnA Maker bot with Python. Click [here][72] to know more about using follow-up prompts to create multiturn conversation. To know more about how to enable and use active learning, click [here][71].

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that uses the [QnA Maker Cognitive AI](https://www.qnamaker.ai) service.

The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes. In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file used as input.

## Concepts introduced in this sample
The [QnA Maker Service][7] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate how to use the [Active Learning][8] to generate suggestions for knowledge base.

# Prerequisites

This samples **requires** prerequisites in order to run.

### Overview

This bot uses [QnA Maker Service](https://www.qnamaker.ai), an AI based cognitive service, to implement simple Question and Answer conversational patterns.

### Create a QnAMaker Application to enable QnA Knowledge Bases

- QnA knowledge base setup and application configuration steps can be found [here](https://aka.ms/qna-instructions).
  - Note that these instructions are for a different sample, but still apply to this sample.  Namely: Create a QnA service, and import 'cognitiveModels/smartLightFAQ.tsv' to a knowledgebase
  - The knowledgebase 'Deployment Details' will show something like:
    ```bash
    POST /knowledgebases/04e394a2-7d8a-4a56-9344-db13fb1d9d79/generateAnswer
    Host: https://servicename.azurewebsites.net/qnamaker
    Authorization: EndpointKey bc3fbc8e-f461-66b1-8c58-4d6799320f9c
    Content-Type: application/json
    {"question":"<Your question>"}
    ```
      - QNA_KNOWLEDGEBASE_ID is '04e394a2-7d8a-4a56-9344-db13fb1d9d79'
      - QNA_ENDPOINT_KEY is 'bc3fbc8e-f461-66b1-8c58-4d6799320f9c'
      - QNA_ENDPOINT_HOST is 'https://servicename.azurewebsites.net/qnamaker'
- Enable [Active Learning][8] on your QnA service.

## Running the sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- Bring up a terminal, navigate to `botbuilder-samples\samples\python\11.qnamaker` folder
- Activate your desired virtual environment
- In the terminal, type `pip install -r requirements.txt`
- Update `QNA_KNOWLEDGEBASE_ID`, `QNA_ENDPOINT_KEY`, and `QNA_ENDPOINT_HOST` in `config.py`
- Run your bot with `python app.py`

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy the bot to Azure
See [Deploy your bot][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][60].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

# Further reading
* [Manage QnA Maker resources](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
* [Use follow-up prompts to create multiple turns of a conversation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/multiturn-conversation)
* [Quickstart: Create, train, and publish your QnA Maker knowledge base](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-publish-knowledge-base)
* [Active learning Documentation][8]
* [Bot Framework Documentation][80]
* [Bot Basics][90]
* [Azure Bot Service Introduction][100]
* [Azure Bot Service Documentation][110]
* [msbot CLI][130]
* [Azure Portal][140]

[1]: https://dev.botframework.com
[2]: https://docs.microsoft.com/en-us/visualstudio/releasenotes/vs2017-relnotes
[4]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://aka.ms/botframeworkemulator
[7]: https://www.qnamaker.ai
[8]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/improve-knowledge-base
[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[60]: https://portal.azure.com
[70]: https://azure.microsoft.com/get-started/
[80]: https://docs.botframework.com
[90]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[100]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[110]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[120]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[130]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[140]: https://portal.azure.com
[150]: https://www.luis.ai

[71]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/improve-knowledge-base
[72]: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/multiturn-conversation
