# Custom Question Answering

Bot Framework v4 Custom question answering bot sample.

This bot has been created using [Bot Framework][BF], it shows how to create a bot that uses the [Custom question answering feature in Language Service][Quickstart].

The [Custom question answering feature in Language Service][LS] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured and unstructured documents or editorial content in minutes. In this sample, we demonstrate how to use the Custom question answering feature in Language Service to answer questions based on a FAQ text file used as input.

## Prerequisites
- This project requires a [Language resource](https://aka.ms/create-language-resource) with Custom question answering enabled.

### Configure knowledge base of the project
- Follow instructions [here][Quickstart] to create a Custom question answering project. You will need this project's name to be used as `ProjectName` in [.env file](.env).
- Visit [Language Studio][LS] and open created project.
- Go to `Edit knowledge base` -> Click on `...` -> Click on `Import questions and answers` -> Click on `Import as TSV`.
- Import [SampleForCQA.tsv](CognitiveModels/SampleForCQA.tsv) file.
- You can test your knowledge base by clicking on `Test` option.
- Go to `Deploy knowledge base` and click on `Deploy`.

### Connect your bot to the project.
Follow these steps to update [.env file](.env).
- In the [Azure Portal][Azure], go to your resource.
- Go to `Keys and Endpoint` under Resource Management.
- Copy one of the keys as value of `LanguageEndpointKey` and Endpoint as value of `LanguageEndpointHostName` in [.env file](.env).
- `ProjectName` is the name of the project created in [Language Studio][LS].

## To try this sample

- Install the Bot Framework Emulator version 4.14.0 or greater from [here][BFE]
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/48.customQABot-all-features`
    ```bash
    cd samples/javascript_nodejs/48.customQABot-all-features
    ```

- Install modules
    ```bash
    npm install
    ```

- Run the sample
    ```bash
    npm start
    ```
- Connect to the bot using Bot Framework Emulator
  1) Launch Bot Framework Emulator
  2) File -> Open Bot
  3) Enter a Bot URL of `http://localhost:3978/api/messages`
 
## Try precise answering
- Try the following utterances:
  1) Accessibility
  2) Register
- You will notice a short answer returned along with a long answer.
- If testing in [Language Studio][LS], you might have to check `Include short answer response` at the top.
- You can disable precise answering by setting `EnablePreciseAnswer` to false in [.env file](.env).
- You can set `DisplayPreciseAnswerOnly` to true in [.env file](.env) to display just precise answers in the response.
- Learn more about [precise answering][PA].

## Deploy the bot to Azure
See [Deploy your C# bot to Azure][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][Azure].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

## Further reading

- [How bots work](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Question Answering Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/overview)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[70]: https://azure.microsoft.com/get-started/

[LS]: https://language.cognitive.azure.com/
[PA]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/concepts/precise-answering
[BF]: https://dev.botframework.com/
[Quickstart]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/quickstart/sdk
[Azure]: https://portal.azure.com
[BFE]: https://github.com/Microsoft/BotFramework-Emulator/releases