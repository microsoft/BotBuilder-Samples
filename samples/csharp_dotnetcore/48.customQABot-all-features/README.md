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
- Follow instructions [here][Quickstart] to create a Custom question answering project. You will need this project's name to be used as `ProjectName` in [appsettings.json](appsettings.json).
- Visit [Language Studio][LS] and open created project.
- Go to `Edit knowledge base` -> Click on `...` -> Click on `Import questions and answers` -> Click on `Import as TSV`.
- Import [SampleForCQA.tsv](CognitiveModels/SampleForCQA.tsv) file.
- You can test your knowledge base by clicking on `Test` option.
- Go to `Deploy knowledge base` and click on `Deploy`.

### Connect your bot to the project.
Follow these steps to update [appsettings.json](appsettings.json).
- In the [Azure Portal][Azure], go to your resource.
- Go to `Keys and Endpoint` under Resource Management.
- Copy one of the keys as value of `LanguageEndpointKey` and Endpoint as value of `LanguageEndpointHostName` in [appsettings.json](appsettings.json).
- `ProjectName` is the name of the project created in [Language Studio][LS].

## To try this sample

- Install the Bot Framework Emulator version 4.14.0 or greater from [here][BFE]
- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/48.customQABot-all-features`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/48.customQABot-all-features` folder
  - Select `CustomQABotAllFeatures.csproj` file
  - Press `F5` to run the project
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
  - `ActiveLearningCardTitle`, `ActiveLearningCardNoMatchText` and `ActiveLearningCardNoMatchResponse` in the card can be changed from [RootDialog.cs](Dialogs/RootDialog.cs).

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
- You can disable precise answering by setting `EnablePreciseAnswer` to false in [appsettings.json](appsettings.json).
- You can set `DisplayPreciseAnswerOnly` to true in [appsettings.json](appsettings.json) to display just precise answers in the response.
- Learn more about [precise answering][PA].

## Query unstructured content
- Go to your project in [Language Studio][LS]. In `Manage sources`, select `+ Add source`.
- Select `URLs` and add `https://www.microsoft.com/en-us/microsoft-365/blog/2022/01/27/from-empowering-frontline-workers-to-accessibility-improvements-heres-whats-new-in-microsoft-365/`. 
- Select **unstructured** in the `Classify file structure` dropdown.
- Try the following utterances:
  1. Frontline workers
  2. Hybrid work solutions
- Make sure answers are returned with a high score.
- To prevent querying unstructured sources, set `IncludeUnstructuredSources` to false in [RootDialog.cs](Dialogs/RootDialog.cs).

## Try Filters
If you want to return answers with only specified metadata, use the following steps:
- Go to your project in [Language Studio][LS]. In `Edit knowledge bases`, under the **Metadata** column, select `+ Add`
- Select a QnA to edit and add a key value pair. Add the key value pair `Language` : `CSharp`, then select `Save changes`.
- Select `Test`, then **Show advanced options**, then select the metadata you just added (`Language : CSharp`).
You can also filter answers using a bot by passing it metadata and/or source filters. To do this, edit line 81 in [RootDialog.cs](Dialogs/RootDialog.cs) to something like the code snippet below. For more information, see [Query filters](https://docs.microsoft.com/en-us/rest/api/cognitiveservices/questionanswering/question-answering/get-answers#queryfilters).
    ```csharp
    var filters = new Filters
    {
        MetadataFilter = new MetadataFilter()
        {
            LogicalOperation = Bot.Builder.AI.QnA.JoinOperator.AND.ToString()
        },
        LogicalOperation = Bot.Builder.AI.QnA.JoinOperator.AND.ToString()
    };
    filters.MetadataFilter.Metadata.Add(new KeyValuePair<string, string>("Language", "CSharp"));
    filters.SourceFilter.Add("SampleForCQA.tsv");
    filters.SourceFilter.Add("SampleActiveLearningImport.tsv");
    
    // Initialize Filters with filters in line No. 81
    ```    

## Microsoft Teams channel group chat fix
To get answers from the service when a bot (named as `HelpBot`) is added to a Teams channel or Teams group chat, refer to it as `@HelpBot` `How to build a bot?`.
However, the bot may try to send `<at>HelpBot</at>` `How to build a bot?` as a query to the Custom question answering service, which may not give expected results for question to bot. The following code removes `<at>HelpBot</at>` mentions of the bot from the message and sends the remaining text as query to the service.
- Goto `Bots/CustomQABot.cs`
- Add the following references:
    ```csharp
    using Microsoft.Bot.Connector;
    using System.Text.RegularExpressions;
    ```
- Modify `OnTurnAsync` function as:
    ```csharp
    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // Teams group chat
            if (turnContext.Activity.ChannelId.Equals(Channels.Msteams))
            {
                turnContext.Activity.Text = turnContext.Activity.RemoveRecipientMention();
            }
            
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    ```

## Deploy the bot to Azure
See [Deploy your C# bot to Azure][50] for instructions.

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