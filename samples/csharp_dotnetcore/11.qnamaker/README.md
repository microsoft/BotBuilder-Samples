This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Core 2. 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question
and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/11.qnamaker` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/tutorials/migrate-knowledge-base) to
import the [smartLightFAQ.tsv](CognitiveModels/smartLightFAQ.tsv) to your newly created QnA Maker service.
- Update [qnamaker.bot](qnamaker.bot) with your kbid (KnowledgeBase Id), hostname, and endpointKey in the "qna" services section. You can find this
information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai)
  - If you changed the `name` property of the `qna` service in your `.bot` file, be sure to update `QnAMakerKey` in `QnABot.cs`
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the
QnA Maker CLI to deploy the model.

# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/11.qnamaker`) and open QnABot.csproj in Visual Studio .
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/11.qnamaker` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator).

### Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/11.qnamaker` folder.
- Select the `qnamaker.bot` file.

## Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete set of deployment instructions.

**Note:** If you already created a QnA Knowledgebase, running `msbot clone services ...` will create an additional one. You can deal with this in two different ways:

1. Remove the entry with `"type": "qna"` from `DeploymentScripts/MsbotClone`. You'll then need to update your `.bot` file with the QnA settings from your previous QnA Knowledgebse and run the `az publish ...` command produced at the end of running the `msbot clone services ...` command.

2. Delete the original QnA Knowledgebase and resources you created earlier. Ensure that you re-import any `.tsv` files, as needed.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
