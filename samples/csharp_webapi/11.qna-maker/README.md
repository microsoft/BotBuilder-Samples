This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Web API. 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question
and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```

## Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/tutorials/migrate-knowledge-base) to import the [smartLightFAQ.tsv](CognitiveModels/smartLightFAQ.tsv) to your newly created QnA Maker service.
- Update [qna-maker.bot](qna-maker.bot) with your `kbid` (KnowledgeBase Id) and `endpointKey` in the "qna" services section. You can find this
information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai)
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the Qna Maker CLI to deploy the model.


## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\samples\csharp_webapi\11.QnAMaker`) and open `QnABot.csproj` in Visual studio 
- Run the project (press `F5` key) 

## Visual Studio Code
- Open `BotBuilder-Samples\samples\csharp_webapi\11.QnAMaker` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\samples\csharp_webapi\11.QnAMaker` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\samples\csharp_webapi\11.QnAMaker` folder.
- Select the `qna-maker.bot` file.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
