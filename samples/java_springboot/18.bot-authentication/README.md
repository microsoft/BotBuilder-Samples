# Bot Authentication

Bot Framework v4 bot authentication sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use authentication in your bot using OAuth.

The sample uses the bot authentication capabilities in [Azure Bot Service](https://docs.botframework.com), providing features to make it easier to develop a bot that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, etc.

NOTE: Microsoft Teams currently differs slightly in the way auth is integrated with the bot. Refer to sample 46.teams-auth.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.
- Deploy your bot to Azure
- [Add Authentication to your bot via Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=aadv2%2Ccsharp)

## To try this sample
- From the root of this project folder:
  - Build the sample using `mvn package`
  - Run it by using `java -jar .\target\bot-authentication-sample.jar`

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Interacting with the bot

This sample uses bot authentication capabilities in Azure Bot Service, providing features to make it easier to develop a bot that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, etc. These updates also take steps towards an improved user experience by eliminating the magic code verification for some clients.

## Deploy the bot to Azure

As described on [Deploy your bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli), you will perform the first 4 steps to setup the Azure app, then deploy the code using the azure-webapp Maven plugin.

### 1. Login to Azure
From a command (or PowerShell) prompt in the root of the bot folder, execute:  
`az login`
  
### 2. Set the subscription
`az account set --subscription "<azure-subscription>"`

If you aren't sure which subscription to use for deploying the bot, you can view the list of subscriptions for your account by using `az account list` command. 

### 3. Create an App registration
`az ad app create --display-name "<botname>" --password "<appsecret>" --available-to-other-tenants`

Replace `<botname>` and `<appsecret>` with your own values.

`<botname>` is the unique name of your bot.  
`<appsecret>` is a minimum 16 character password for your bot. 

Record the `appid` from the returned JSON

### 4. Create the Azure resources
Replace the values for `<appid>`, `<appsecret>`, `<botname>`, and `<groupname>` in the following commands:

#### To a new Resource Group
```
az deployment sub create --name "authenticationBotDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="authenticationBotPlan" newWebAppName="authenticationBot" groupLocation="westus" newAppServicePlanLocation="westus"
``` 

#### To an existing Resource Group
```
az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="authenticationBot" newAppServicePlanName="authenticationBotPlan" appServicePlanLocation="westus" --name "authenticationBot"
```

### 5. Update app id and password
In src/main/resources/application.properties update 
  - `MicrosoftAppPassword` with the botsecret value
  - `MicrosoftAppId` with the appid from the first step

### 6. Deploy the code
- Execute `mvn clean package` 
- Execute `mvn azure-webapp:deploy -Dgroupname="<groupname>" -Dbotname="<bot-app-service-name>"`

If the deployment is successful, you will be able to test it via "Test in Web Chat" from the Azure Portal using the "Bot Channel Registration" for the bot.

After the bot is deployed, you only need to execute #6 if you make changes to the bot.

## Further reading
- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://docs.microsoft.com/en-us/java/api/overview/azure/maven/azure-webapp-maven-plugin/readme?view=azure-java-stable)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Azure Portal](https://portal.azure.com)
- [Add Authentication to Your Bot Via Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=csharp)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)