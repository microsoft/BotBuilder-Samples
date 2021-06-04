# Authentication Bot Utilizing MS Graph

Bot Framework v4 bot authentication using Microsoft Graph sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), is shows how to use the bot authentication capabilities of Azure Bot Service. In this sample we are assuming the OAuth 2 provider is Azure Active Directory v2 (AADv2) and are utilizing the Microsoft Graph API to retrieve data about the user. [Check here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=csharp) for information about getting an AADv2
application setup for use in Azure Bot Service. The [scopes](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) used in this sample are the following:

- `openid`
- `profile`
- `User.Read`

NOTE: Microsoft Teams currently differs slightly in the way auth is integrated with the bot. Refer to sample 46.teams-auth.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure. 
- Update `application.properties` with required configuration settings
  - MicrosoftAppId 
  - MicrosoftAppPassword
  - ConnectionName

## To try this sample
- From the root of this project folder:
  - Build the sample using `mvn package`
  - Run it by using `java -jar .\target\bot-authentication-msgraph-sample.jar`

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)
- In Bot Framework Emulator Settings, enable `Use a sign-in verification code for OAuthCards` to receive the magic code

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Interacting with the bot

This sample uses the bot authentication capabilities of Azure Bot Service, providing features to make it easier to develop a bot that
authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, and so on. These updates also
take steps towards an improved user experience by eliminating the magic code verification for some clients and channels.
It is important to note that the user's token does not need to be stored in the bot. When the bot needs to use or verify the user has a valid token at any point the OAuth prompt may be sent. If the token is not valid they will be prompted to login.

## Microsoft Graph API

This sample demonstrates using Azure Active Directory v2 as the OAuth2 provider and utilizes the Microsoft Graph API.
Microsoft Graph is a Microsoft developer platform that connects multiple services and devices. Initially released in 2015,
the Microsoft Graph builds on Office 365 APIs and allows developers to integrate their services with Microsoft products including Windows, Office 365, and Azure.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.
## GraphError 404: ResourceNotFound, Resource could not be discovered

This error may confusingly present itself if either of the following are true:

- You're using an email ending in `@microsoft.com`, and/or
- Your OAuth AAD tenant is `microsoft.onmicrosoft.com`.

## Further reading

- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://github.com/microsoft/azure-maven-plugins/tree/develop/azure-webapp-maven-plugin)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Microsoft Graph API](https://developer.microsoft.com/en-us/graph)
- [MS Graph Docs](https://developer.microsoft.com/en-us/graph/docs/concepts/overview) and [SDK](https://github.com/microsoftgraph/msgraph-sdk-java)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
