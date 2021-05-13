# SimpleBotToBot Echo Skill

Bot Framework v4 skills echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple skill consumer (SimpleRootBot) that sends message activities to a skill (EchoSkillBot) that echoes it back.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.

## Key concepts in this sample

The solution includes a parent bot (`SimpleRootBot`) and a skill bot (`EchoSkillBot`) and shows how the parent bot can post activities to the skill bot and returns the skill responses to the user.

- `SimpleRootBot`: this project shows how to consume an echo skill and includes:
  - A [RootBot](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/RootBot.java) that calls the echo skill and keeps the conversation active until the user says "end" or "stop". [RootBot](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/RootBot.java) also keeps track of the conversation with the skill and handles the `EndOfConversation` activity received from the skill to terminate the conversation
  - A simple [SkillConversationIdFactory](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/SkillConversationIdFactory.java) based on an in memory `Map` that creates and maintains conversation IDs used to interact with a skill
  - A [SkillsConfiguration](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/SkillsConfiguration.java) class that can load skill definitions from `appsettings`
  - A [SkillController](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/controller/SkillController.java) that handles skill responses
  - An [AllowedSkillsClaimsValidator](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/Authentication/AllowedSkillsClaimsValidator.java) class that is used to authenticate that responses sent to the bot are coming from the configured skills
  - A [Application](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/Application.java) class that shows how to register the different skill components for dependency injection
- `EchoSkillBot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project includes:
  - A sample [EchoBot](EchoSkillBot/src/main/java/com/microsoft/echoskillbot/EchoBot.java) that shows how to send EndOfConversation based on the message sent to the skill and yield control back to the parent bot
  - A sample [AllowedCallersClaimsValidator](EchoSkillBot/src/main/java/com/microsoft/echoskillbot/authentication/AllowedCallersClaimsValidator.java) that shows how validate that the skill is only invoked from a list of allowed callers
  - A [sample skill manifest](EchoSkillBot/src/main/webapp/manifest/echoskillbot-manifest-1.0.json) that describes what the skill can do

## To try this sample
- Create a bot registration in the azure portal for the `EchoSkillBot` and update [EchoSkillBot/application.properties](EchoSkillBot/src/main/resources/application.properties) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `SimpleRootBot` and update [SimpleRootBot/application.properties](SimpleRootBot/src/main/resources/application.properties) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [SimpleRootBot/application.properties](SimpleRootBot/src/main/resources/application.properties) with the app ID for the skill you created in the previous step
- (Optionally) Add the `SimpleRootBot` `MicrosoftAppId` to the `AllowedCallers` list in [EchoSkillBot/application.properties](EchoSkillBot/src/main/resources/application.properties) 
- Open the `SimpleBotToBot` project and start it for debugging
- Open the `EchoSkillsBot` project and start it for debugging

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `SimpleRootBot`

## Deploy the bot to Azure

As described on [Deploy your bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli), you will perform the first 4 steps to setup the Azure app, then deploy the code using the azure-webapp Maven plugin.

These steps should be followed to deploy the DialogRootBot and DialogSkillBot.

### 1. Login to Azure

From a command (or PowerShell) prompt in the root of the bot folder, execute:
`az login`

### 2. Set the subscription

```
az account set --subscription "<azure-subscription>"
```

If you aren't sure which subscription to use for deploying the bot,  you can view the list of subscriptions for your account by using `az account list` command.

### 3. Create an App registration

```
az ad app create --display-name "<botname>" --password "<appsecret>" --available-to-other-tenants
```

Replace `<botname>` and `<appsecret>` with your own values.

`<botname>` is the unique name of your bot.
`<appsecret>` is a minimum 16 character password for your bot.

Record the `appid` from the returned JSON

### 4. Create the Azure resources

Replace the values for `<appid>`, `<appsecret>`, `<botname>`, and `<groupname>` in the following commands:

#### To a new Resource Group

```
az deployment sub create --name "botDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="botPlan" newWebAppName="bot" groupLocation="westus" newAppServicePlanLocation="westus"
```

#### To an existing Resource Group

```
az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="bot" newAppServicePlanName="botPlan" appServicePlanLocation="westus" --name "bot"
```

### 5. Update app id and password

In src/main/resources/application.properties update

- `MicrosoftAppPassword` with the botsecret value
- `MicrosoftAppId` with the appid from the first step

### 6. Deploy the code

- Execute `mvn clean package`
- Execute `mvn azure-webapp:deploy -Dgroupname="<groupname>" -Dbotname="<bot-app-service-name>"`

If the deployment is successful, you will be able to test it via  "Test in Web Chat" from the Azure Portal using the "Bot Channel  Registration" for the bot.

After the bot is deployed, you only need to execute #6 if you make changes to the bot.
