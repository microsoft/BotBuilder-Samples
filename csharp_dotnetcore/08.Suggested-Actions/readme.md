This sample demonstrates the use of suggested actions.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Suggested actions
Suggested actions enable your bot to present buttons that the user can tap to provide input. Suggested actions appear close to the composer and enhance
user experience by enabling the user to answer a question or make a selection with a simple tap of a button, rather than having to type a response with
a keyboard. Unlike buttons that appear within rich cards (which remain visible and accessible to the user even after being tapped), buttons that appear
within the suggested actions pane will disappear after the user makes a selection. This prevents the user from tapping stale buttons within a conversation
and simplifies bot development (since you will not need to account for that scenario).
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
 ### Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\8.Suggested-Actions\Suggested_Actions`) and open Suggested_Actions.csproj in Visual Studio 
- Hit F5
 ### Visual studio code
- Open `BotBuilder-Samples\csharp_dotnetcore\8.Suggested-Actions\Suggested_Actions` folder
- Bring up a terminal, navigate to BotBuilder-Samples\csharp_dotnetcore\8.Suggested-Actions\Suggested_Actions
- Type 'dotnet run'.
## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Builder.Dialogs`, and `Microsoft.Bot.Builder.Integration.AspNet.Core`.
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Suggested actions](https://docs.microsoft.com/en-us/azure/bot-service/nodejs/bot-builder-nodejs-send-suggested-actions?view=azure-bot-service-4.0)