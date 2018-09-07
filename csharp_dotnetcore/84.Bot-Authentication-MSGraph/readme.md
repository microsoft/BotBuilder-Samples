This sample uses the bot authentication capabilities of Azure Bot Service. In this sample we are assuming the OAuth 2 provider
is Azure Active Directory v2 (AADv2) and are utilizing the Microsoft Graph API to retrieve data about the
user. [Check here for](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-authentication?view=azure-bot-service-4.0) information about getting an AADv2
application setup for use in Azure Bot Service.
The [scopes](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) used in this sample are
'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'.
# Concepts introduced in this sample
## What is a bot?
A bot is an app that users interact with in a conversational way using text, graphics (cards), or speech. It may be a simple question and answer dialog,
or a sophisticated bot that allows people to interact with services in an intelligent manner using pattern matching,
state tracking and artificial intelligence techniques well-integrated with existing business services.
## Authentication
This sample uses the bot authentication capabilities of Azure Bot Service, providing features to make it easier to develop a bot that
authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, and so on. These updates also
take steps towards an improved user experience by eliminating the magic code verification for some clients and channels.
It is important to note that the user's token does not need to be stored in the bot. When the bot needs to use or verify the user has a valid token at any point
the OAuth prompt may be sent. If the token is not valid they will be prompted to login.
## Microsoft Graph API
This sample demonstrates using Azure Active Directory v2 as the OAuth2 provider and utilizes the Microsoft Graph API.
Microsoft Graph is a Microsoft developer platform that connects multiple services and devices. Initially released in 2015,
the Microsoft Graph builds on Office 365 APIs and allows developers to integrate their services with Microsoft products
including Windows, Office 365, and Azure.
## To try this sample
- Clone the repository.
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
 ### Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\19.Bot-Authentication`) and open Handling_Attachments.csproj in Visual Studio 
- Hit F5
 ### Visual Studio Code
- Open `BotBuilder-Samples\csharp_dotnetcore\19.Bot-Authentication\` folder
- Bring up a terminal, navigate to BotBuilder-Samples\csharp_dotnetcore\19.Bot-Authentication
- Type 'dotnet run'.
## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Builder.Dialogs`, `Microsoft.Bot.Connector`, and `Microsoft.Bot.Builder.Integration.AspNet.Core`.
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Microsoft Graph API](https://developer.microsoft.com/en-us/graph)