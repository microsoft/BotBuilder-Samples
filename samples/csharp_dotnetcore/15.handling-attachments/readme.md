This sample demonstrates different ways a bot may send and receive attachments.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Attachments
A message exchange between user and bot may contain cards and media attachments, such as images, video, audio, and files.
The types of attachments that may be sent and recieved varies by channel. Additionally, a bot may also recieve file attachments.
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/15.handling-attachments` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/15.handling-attachments`) and open HandlingAttachmentsBot.csproj in Visual Studio 
- Hit F5
## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/15.handling-attachments` folder
- Bring up a terminal, navigate to botbuilder-samples/samples/csharp_dotnetcore/15.handling-attachments
- Type 'dotnet run'.
## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Connector`, and `Microsoft.Bot.Builder.Integration.AspNet.Core`.
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity Processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Attachments](https://docs.microsoft.com/en-us/azure/bot-service/nodejs/bot-builder-nodejs-send-receive-attachments?view=azure-bot-service-4.0)
