# QnA Maker

Bot Framework v4 Custom Question Answering bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that uses the [Custom question answering feature in Language Service](https://language.cognitive.azure.com) service.

The [Custom question answering feature in Language Service](https://language.cognitive.azure.com) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes. In this sample, we demonstrate how to use the Custom question answering feature in Language Service to answer questions based on a FAQ text file used as input.

## Prerequisites

This samples **requires** prerequisites in order to run.

### Overview

- This bot uses [Custom question answering feature in Language Service](https://language.cognitive.azure.com), an AI based cognitive service, to implement simple Question and Answer conversational patterns.

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

### Create a Custom Question Answering Project from your own content

Step by step guide to create your first Custom Question Answering project can be found [here](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/quickstart/sdk).

### Obtain values to connect your bot to the knowledge base

- In the [Azure Portal](https://ms.portal.azure.com/), go to your resource.
- Go to Keys and Endpoint under Resource Management.
- Record one of your keys and your Endpoint.

### Update the settings file

- Fill in QnAKnowledgebaseId, which is the name of your project in [Language Studio](https://language.cognitive.azure.com/questionAnswering/projects).
- QnAEndpointKey would be one of the keys and QnAEndpointHostName is your Endpoint from [Azure Portal](https://ms.portal.azure.com/).

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/12.customQABot`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/12.customQABot` folder
  - Select `CustomQABot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Interacting with the bot

Question answering provides cloud-based Natural Language Processing (NLP) that allows you to create a natural conversational layer over your data. It is used to find the most appropriate answer for any input from your custom knowledge base (KB) of information.

One of the basic requirements in writing your own bot is to seed it with questions and answers. In many cases, the questions and answers already exist in content like FAQ URLs/documents, product manuals, etc.  Using Custom Question Answering, users can customize different aspects like edit question and answer pairs extracted from the content source, define synonyms and metadata, accept question suggestions etc. Custom Question Answering uses machine learning to extract relevant question-answer pairs from your content. It uses state-of-the-art transformer models and Turing natural language model, with powerful matching and ranking algorithms to provide the best possible match between the user query and the questions.

 Using this capability users can customize different aspects like edit question and answer pairs extracted from the content source, define synonyms and metadata, accept question suggestions etc.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Question Answering Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/overview)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [QnA Maker CLI](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
