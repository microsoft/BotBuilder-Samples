# Custom Question Answering

This is the Bot Framework v4 Custom question answering bot sample, which shows how to use advanced features of [Cognitive Services question answering][LS], such as [Precise answering][PA], support for unstructured sources, [Multi-turn conversations][MT], and [Active Learning][AL] in a bot.

This bot was created using [Bot Framework][BF].

## Concepts introduced in this sample
[Question answering][LS] enables you to build, train, and publish a simple question and answer bot based on FAQ URLs, structured and unstructured documents, or editorial content in minutes. In this sample, we demonstrate:
- How to use active learning to generate suggestions for your knowledge base.
- How to use follow-up prompts to create multiple turns of a conversation.
- How to configure display of precise answers.
- How to enable/disable querying unstructured sources with the bot.

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

## Try Active Learning
- Try the following utterances:
  1. Surface Book
  2. Power
- In Language Studio, select `inspect` to view the scores of the returned answers and compare how close they are.
- In [Bot Framework Emulator][BFE], a card is generated with the suggestions.
  - Clicking an option sends a [feedback record](https://docs.microsoft.com/en-us/rest/api/cognitiveservices/questionanswering/question-answering-projects/add-feedback), which shows as suggestion under `Review suggestions` in [Language Studio][LS].
  - `ACTIVE_LEARNING_CARD_TITLE`, `ACTIVE_LEARNING_CARD_NO_MATCH_TEXT` and `ACTIVE_LEARNING_CARD_NO_MATCH_RESPONSE` in the card can be changed from [rootDialog.js](dialogs/rootDialog.js).

## Try Multi-turn prompt
- Try the following utterances:
  1. Accessibility
  2. Options
- You'll notice that multi-turn prompts associated with the question are also returned in the responses.

## Try Precise Answering
- Try the following utterances:
  1) Accessibility
  2) Register
- You will notice a short answer returned along with a long answer.
- If testing in [Language Studio][LS], you might have to check `Include short answer response` at the top.
- You can disable precise answering by setting `EnablePreciseAnswer` to false in [.env file](.env).
- You can set `DisplayPreciseAnswerOnly` to true in [.env file](.env) to display just precise answers in the response.
- Learn more about [precise answering][PA].

## Query unstructured content
- Go to your project in [Language Studio][LS]. In `Manage sources`, select `+ Add source`.
- Select `URLs` and add `https://www.microsoft.com/en-us/microsoft-365/blog/2022/01/27/from-empowering-frontline-workers-to-accessibility-improvements-heres-whats-new-in-microsoft-365/`. 
- Select **unstructured** in the `Classify file structure` dropdown.
- Try the following utterances:
  1. Frontline workers
  2. Hybrid work solutions
- Make sure answers are returned with a high score.
- To prevent querying unstructured sources, set `INCLUDE_UNSTRUCTURED_SOURCES` to false in [rootDialog.js](dialogs/rootDialog.js).

## Try Filters
If you want to return answers with only specified metadata, use the following steps:
- Go to your project in [Language Studio][LS]. In `Edit knowledge bases`, under the **Metadata** column, select `+ Add`
- Select a QnA to edit and add a key value pair. Add the key value pair `Language` : `Javascript`, then select `Save changes`.
- Select `Test`, then **Show advanced options**, then select the metadata you just added (`Language : Javascript`).
You can also filter answers using a bot by passing it metadata and/or source filters. To do this, edit line 54 in [rootDialog.js](dialogs/rootDialog.js) to something like the code snippet below. For more information, see [Query filters](https://docs.microsoft.com/en-us/rest/api/cognitiveservices/questionanswering/question-answering/get-answers#queryfilters).
    ```js
    //Add below lines in line no. 54 in rootDialog.js
    var filters = {
        metadataFilter: {
            metadata: [
                { key: 'Language', value: 'Javascript' },
            ],
            logicalOperation: 'OR'
        },
        sourceFilter: [],
        logicalOperation: 'AND'
    };

    qnaMakerDialog.filters = filters;
    ```    

## Microsoft Teams channel group chat fix
To get answers from the service when a bot (named as `HelpBot`) is added to a Teams channel or Teams group chat, refer to it as `@HelpBot` `How to build a bot?`.
However, the bot may try to send `<at>HelpBot</at>` `How to build a bot?` as a query to the Custom question answering service, which may not give expected results for question to bot. The following code removes `<at>HelpBot</at>` mentions of the bot from the message and sends the remaining text as query to the service.
- Goto `dialogs/rootDialog.js`
- Add the following references:
    ```js
    const {
        TurnContext
    } = require('botbuilder-core');
    ```
- Modify `run` function as
    ```js
    async run(context, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(context);

        if (context.activity.channelId === "msteams") {
            context.activity.text = TurnContext.removeRecipientMention(context.request);
        }

        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }
    ```

## Deploy the bot to Azure
See [Deploy your bot to Azure][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][Azure].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

## Further reading
- [How bots work][90]
- [Question Answering Documentation](https://docs.microsoft.com/azure/cognitive-services/language-service/question-answering/overview)
- [Channels and Bot Connector Service](https://docs.microsoft.com/azure/bot-service/bot-concepts)
- [Active learning Documentation][AL]
- [Multi-turn Conversations][MT]
- [Precise Answering][PA]

[50]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-deploy-azure
[70]: https://azure.microsoft.com/get-started/
[90]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics
[100]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction
[110]: https://docs.microsoft.com/azure/bot-service/
[140]: https://portal.azure.com

[LS]: https://language.cognitive.azure.com/
[MT]: https://docs.microsoft.com/azure/cognitive-services/language-service/question-answering/tutorials/guided-conversations
[AL]: https://docs.microsoft.com/azure/cognitive-services/language-service/question-answering/tutorials/active-learning
[PA]: https://docs.microsoft.com/azure/cognitive-services/language-service/question-answering/concepts/precise-answering
[BF]: https://dev.botframework.com/
[Quickstart]: https://docs.microsoft.com/azure/cognitive-services/language-service/question-answering/quickstart/sdk
[Azure]: https://portal.azure.com/
[BFE]: https://github.com/Microsoft/BotFramework-Emulator/releases