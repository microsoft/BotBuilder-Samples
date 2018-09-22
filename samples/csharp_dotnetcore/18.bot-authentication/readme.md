This sample demonstrates the use of OAuth within a bot.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Authentication
This Sample uses bot authentication capabilities in Azure Bot Service, providing features to make it easier to develop a bot
that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, and so on. These
updates also take steps towards an improved user experience by eliminating the magic code verification for some clients.
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
 ### Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\19.Bot-Authentication`) and open Bot_Authentication.csproj in Visual Studio 
- Hit F5
 ### Visual Studio Code
- Open `BotBuilder-Samples\csharp_dotnetcore\7.Using-Adaptive-Cards` folder
- Bring up a terminal, navigate to BotBuilder-Samples\csharp_dotnetcore\19.Bot-Authentication
- Type 'dotnet run'.
## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Builder.Dialogs`, and `Microsoft.Bot.Builder.Integration.AspNet.Core`.
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Bot Authorization](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-authentication?view=azure-bot-service-4.0)