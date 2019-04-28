# Authentication Bot Utilizing MS Graph

Bot Framework v4 bot authentication using Microsoft Graph sample

This bot has been created using [Bot Framework](https://dev.botframework.com), is shows how to uss the bot authentication capabilities of Azure Bot Service. In this sample we are assuming the OAuth 2 provider is Azure Active Directory v2 (AADv2) and are utilizing the Microsoft Graph API to retrieve data about the user. [Check here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=csharp) for information about getting an AADv2
application setup for use in Azure Bot Service. The [scopes](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) used in this sample are the following:

- `email`
- `Mail.Read`
- `Mail.Send.Shared`
- `openid`
- `profile`
- `User.Read`
- `User.ReadBasic.All`

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- Update `appsettings.json` with required configuration settings
  - MicrosoftAppId
  - MicrosoftAppPassword
  - ConnectionName

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/24.bot-authentication-msgraph`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/24.bot-authentication-msgraph` folder
  - Select `BotAuthenticationMSGraph.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)
- In Bot Framework Emulator Settings, enable `Use a sign-in verification code for OAuthCards` to receive the magic code

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Authentication

This sample uses the bot authentication capabilities of Azure Bot Service, providing features to make it easier to develop a bot that
authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, and so on. These updates also
take steps towards an improved user experience by eliminating the magic code verification for some clients and channels.
It is important to note that the user's token does not need to be stored in the bot. When the bot needs to use or verify the user has a valid token at any point the OAuth prompt may be sent. If the token is not valid they will be prompted to login.

## Microsoft Graph API

This sample demonstrates using Azure Active Directory v2 as the OAuth2 provider and utilizes the Microsoft Graph API.
Microsoft Graph is a Microsoft developer platform that connects multiple services and devices. Initially released in 2015,
the Microsoft Graph builds on Office 365 APIs and allows developers to integrate their services with Microsoft products
including Windows, Office 365, and Azure.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Microsoft Graph API](https://developer.microsoft.com/en-us/graph)
- [MS Graph Docs](https://developer.microsoft.com/en-us/graph/docs/concepts/overview) and [SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
