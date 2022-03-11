# Custom Question Answering

Bot Framework v4 Custom question answering bot sample. This sample shows how to integrate Multiturn and Active learning in a Custom Question Answering bot with ASP.Net Core-2. Click [here](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/guided-conversations) to know more about using follow-up prompts to create multiturn conversation. To know more about how to enable and use active learning, click [here](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/active-learning).

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that uses the [Custom question answering feature in Language Service](https://language.cognitive.azure.com) service.

The [Custom question answering feature in Language Service](https://language.cognitive.azure.com) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes. In this sample, we demonstrate how to use the Custom question answering service to answer questions based on a structured, semi-structured or an unstructured source as input.

## Concepts introduced in this sample
The [Custom question answering feature in Language Service](https://language.cognitive.azure.com) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate 
- How to use the Active Learning to generate suggestions for knowledge base.
- How to use follow up prompts to create multiple turns of a conversation.
- How to configure display of precise answers.
- How to enable/disable querying unstructured sources with the bot.

# Prerequisites
- Create a [Language resource](https://aka.ms/create-language-resource) with Custom question answering enabled.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/quickstart/sdk) to create a Custom question answering project. You will need this project's name to be used as `QnAKnowledgebaseId` in [appsettings.json](appsettings.json).

### Obtain values to connect your bot to the knowledge base
- Follow these steps to update [appsettings.json](appsettings.json).
- In the [Azure Portal](https://ms.portal.azure.com/), go to your resource.
- Go to Keys and Endpoint under Resource Management.
- `QnAEndpointKey` would be one of the keys and `QnAEndpointHostName` would be the Endpoint from [Azure Portal](https://ms.portal.azure.com/).
- `QnAKnowledgebaseId` would be the name of your project in [Language Studio](https://language.cognitive.azure.com/questionAnswering/projects).

# Configure knowledgebase of the project
- Visit [Language Studio](https://language.cognitive.azure.com/) and open created project.
- Go to `Edit knowledge base` -> Click on `...` -> Click on `Import questions and answers` -> Click on `Import as TSV`.
- Import [Sample_qnas_for_CQA.tsv](CognitiveModels/Sample_qnas_for_CQA.tsv) file.
- You can test your bot by clicking on `test` option.
- Go to `Deploy knowledge base` and click on `Deploy`.

## To try this sample

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

# Try Active Learning
- Once your Custom question answering feature in Language Service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample queries:
  1) Surface
  2) Features
- You can observe that, Multiple answers are returned with high score.

# Try Multi-turn prompt
- Once your Custom question answering feature in Language Service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample queries:
  1) Accessibility
  2) Options
- You can notice a prompt, included as part of answer to query.

# Try Precise Answering
- Make sure `EnablePreciseAnswer` is set to true in [appsettings.json](appsettings.json).
- Sample queries:
  1) Accessibility
  2) Register
- You can notice a short answer returned along with a long answer.
- You can set `DisplayPreciseAnswerOnly` in [appsettings.json](appsettings.json) to true to display just precise answers in the response.

# Query unstructured content
- Once your Custom question answering feature in Language Service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Make sure `IncludeUnstructuredSources` is set to true in [RootDialog.cs](Dialog/RootDialog.cs).
- Sample queries:
  1) Frontline workers
  2) Hybrid work solutions
- You can observe that, answers are returned with high score.

##### Microsoft Teams channel group chat fix
- Goto `Bot/QnABot.cs`
- Add References
    ~~~
    using Microsoft.Bot.Connector;
    using System.Text.RegularExpressions;
    ~~~
- Modify `OnTurnAsync` function as:
    ~~~
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
    ~~~

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy the bot to Azure
See [Deploy your C# bot to Azure][50] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][60].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

# Further reading
* [Bot Framework Documentation][80]
* [Bot Basics][90]
* [Azure Bot Service Introduction][100]
* [Azure Bot Service Documentation][110]
* [msbot CLI][130]
* [Azure Portal][140]
* [Active learning Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/active-learning)
* [Multi-turn Conversations](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/tutorials/guided-conversations)
* [Precise Answering](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/concepts/precise-answering)

[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[60]: https://portal.azure.com
[70]: https://azure.microsoft.com/get-started/
[80]: https://docs.botframework.com
[90]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[100]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[110]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[130]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[140]: https://portal.azure.com