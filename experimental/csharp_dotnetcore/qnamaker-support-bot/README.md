# Support Bot in QnA Maker
This sample demonstrates how to create a multiturn support bot with ASP.Net Core 2.

This sample shows how to:
Use QnA Maker to create a multiturn support bot.It also uses active learning feature of QnA Maker to generate suggestions and use them to further train the knowledge base. 

It uses application insights for logging details.

This sample can also optionally use [Personality chat][160] to chat with the user if no match is found in QnA Maker. 

It can optionally use [LUIS][150] model to get the trained intent.

# To try this sample
- Clone the samples repository
- Update BotConfiguration.bot file with your kbid (KnowledgeBase Id) and endpointKey in the "qna" services section. You can find this information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai)
- Update BotConfiguration.bot file with Application insights instrumentation keys and app id. Refer [here][170] for details.
- [Optional] If you want to use LUIS, add the following to the BotConfiguration.bot file and update the LUIS details. Refer [here][180] for details.
{
      "type": "luis",
      "name": "LuisBot",
      "appId": "<Your App Id>",
      "version": "0.1",
      "authoringKey": "<Your Authoring Key>",
      "subscriptionKey": "<Your Subscription Key>",
      "region": "<Your region>",
      "id": "158"
    },
    Also, add EnableLuis = true in Models/Constants.cs file
- [Optional] If you are using [personality chat][160], add this line to appsettings.json file:
"personalityChatKey": "<Your PersonalityChat Key>" and make EnablePersonalityChat = true in Models/Constants.cs file
- [Optional] Update the appsettings.json file under BotBuilder-Samples/experimental/csharp_dotnetcore/qnamaker-activelearning-bot/ with your botFileSecret. For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
to create a QnA Maker service.
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the
QnA Maker CLI to deploy the model.

# Running Locally

## Visual Studio
- Open qnamaker-activelearning-bot.sln in Visual Studio.
- Run the project (press `F5` key).

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the [Bot Framework emulator][6].

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and open [qnamaker-activelearning.bot](qnamaker-activelearning.bot).

# Try Active Learning
- Once your QnA Maker service is up and you have published the sample KB, try the following queries to trigger the Train API on the bot.
- Sample queries: "surface pro 4", "apps on your surface pro"

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

[1]: https://dev.botframework.com
[2]: https://docs.microsoft.com/en-us/visualstudio/releasenotes/vs2017-relnotes
[3]: https://dotnet.microsoft.com/download/dotnet-core/2.1
[4]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://aka.ms/botframeworkemulator
[7]: https://www.qnamaker.ai
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
[160]: https://labs.cognitive.microsoft.com/en-us/project-personality-chat
[170]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-resources-app-insights-keys?view=azure-bot-service-4.0
[180]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys 
