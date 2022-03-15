# Custom Question Answering

Bot Framework v4 Custom question answering bot sample. This sample demonstrates usage of advanced features of Custom question answering like [Precise answering][PA], support for unstructured sources along with [Multi-turn conversations][MT] and [Active Learning][AL] in a bot.

This bot has been created using [Bot Framework][BF], it shows how to create a bot that uses the [Custom question answering feature in Language Service][LS].

The [Custom question answering feature in Language Service][LS] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes. In this sample, we demonstrate how to use the Custom question answering service to answer questions based on a structured, semi-structured or an unstructured source as input.

## Concepts introduced in this sample
The [Custom question answering feature in Language Service][LS] enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate 
- How to use the Active Learning to generate suggestions for knowledge base.
- How to use follow up prompts to create multiple turns of a conversation.
- How to configure display of precise answers.
- How to enable/disable querying unstructured sources with the bot.

# Prerequisites
- Create a [Language resource](https://aka.ms/create-language-resource) with Custom question answering enabled.
- Follow instructions [here][Quickstart] to create a Custom question answering project. You will need this project's name to be used as `QnAKnowledgebaseId` in [appsettings.json](appsettings.json).

### Update [appsettings.json](appsettings.json) to connect your bot to the knowledge base
- In the [Azure Portal][Azure], go to your resource.
- Go to Keys and Endpoint under Resource Management.
- `QnAEndpointKey` would be one of the keys and `QnAEndpointHostName` would be the Endpoint from [Azure Portal][Azure].
- `QnAKnowledgebaseId` would be the name of your project.

# Configure knowledge base of the project
- Visit [Language Studio][LS] and open created project.
- Go to `Edit knowledge base` -> Click on `...` -> Click on `Import questions and answers` -> Click on `Import as TSV`.
- Import [Sample_qnas_for_CQA.tsv](CognitiveModels/Sample_qnas_for_CQA.tsv) file.
- You can test your bot by clicking on `Test` option.
- Go to `Deploy knowledge base` and click on `Deploy`.

# To try this sample

- Install the Bot Framework Emulator version 4.14.1 or greater from [here][BFE]
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

# Try Active Learning
- Try the following queries:
  1) Surface
  2) Features
- In Language Studio, click on inspect to see closeness in scores of returned answers.
- In [Bot Framework Emulator][BFE], a card is generated with suggestions shown.
  - Clicking an option would send a [feedback record](https://docs.microsoft.com/en-us/rest/api/cognitiveservices/questionanswering/question-answering-projects/add-feedback) which would show as suggestion under `Review suggestions` in [Language Studio][LS].
  - `DefaultCardTitle`, `DefaultCardNoMatchText` and `DefaultCardNoMatchResponse` in the card could be changed from [RootDialog.cs](Dialogs/RootDialog.cs).

# Try Multi-turn prompt
- Try the following queries:
  1) Accessibility
  2) Options
- You will notice that multi-turn prompts associated with the question are also returned with response.

# Try Precise Answering
- Try the following queries:
  1) Accessibility
  2) Register
- You can notice a short answer returned along with a long answer.
- If testing in [Language Studio][LS], you might have to check `Include short answer response` at the top.
- You can disable precise answering by setting `EnablePreciseAnswer` to false in [appsettings.json](appsettings.json).
- You can set `DisplayPreciseAnswerOnly` in [appsettings.json](appsettings.json) to true to display just precise answers in the response.

# Query unstructured content
- Go to your project in [Language Studio][LS] -> In `Manage sources` click on `+ Add source`
- Click on `URLs` and add [this](https://www.microsoft.com/en-us/microsoft-365/blog/2022/01/27/from-empowering-frontline-workers-to-accessibility-improvements-heres-whats-new-in-microsoft-365/) link by classifying it as **unstructured** under `Classify file structure`.
- Sample queries:
  1) Frontline workers
  2) Hybrid work solutions
- You can observe that, answers are returned with high score.
- You can set `IncludeUnstructuredSources` to false in [RootDialog.cs](Dialogs/RootDialog.cs) to prevent querying unstructured sources.

# Microsoft Teams channel group chat fix
When a bot (named as `HelpBot`) is added to a Teams channel or Teams group chat, you will have to refer it as `@HelpBot question to bot` to get answers from the service.
However, bot tries to send `<at>HelpBot</at> question to bot` as query to Custom question answering service which may not give expected results for question to bot. The following code removes <at>HelpBot</at> mentions of the bot from the message and sends the remaining text as query to the service.
- Goto `Bot/CustomQABot.cs`
- Add References
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

# Deploy the bot to Azure
See [Deploy your C# bot to Azure][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][Azure].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

# Further reading
* [Bot Framework Documentation][80]
* [Bot Basics][90]
* [Azure Bot Service Introduction][100]
* [Azure Bot Service Documentation][110]
* [Azure Portal][Azure]
* [Active learning Documentation][AL]
* [Multi-turn Conversations][MT]
* [Precise Answering][PA]

[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[70]: https://azure.microsoft.com/get-started/
[80]: https://docs.botframework.com
[90]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[100]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[110]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[140]: https://portal.azure.com

[LS]: https://language.cognitive.azure.com/
[MT]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/guided-conversations
[AL]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/active-learning
[PA]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/concepts/precise-answering
[BF]: https://dev.botframework.com/
[Quickstart]: https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/quickstart/sdk
[Azure]: https://portal.azure.com/
[BFE]: https://github.com/Microsoft/BotFramework-Emulator/releases