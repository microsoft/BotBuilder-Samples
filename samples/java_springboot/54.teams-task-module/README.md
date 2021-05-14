# Teams Task Module

Bot Framework v4 Teams Task Module sample.

This bot has been created using [Bot Framework](https://dev.botframework.com). It shows how to fetch a Task Module from a Hero Card button and receive input from an Adaptive Card in the Task Module.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install Maven
- An account on Azure if you want to deploy to Azure.
- Microsoft Teams is installed and you have an account
- [ngrok](https://ngrok.com/) or equivalent tunnelling solution

## To try this sample

> Note these instructions are for running the sample on your local machine, the tunnelling solution is required because
the Teams service needs to call into the bot.

- Run ngrok http -host-header=rewrite 3978

- Create [Bot Framework registration resource](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart-registration) in Azure
    - Use the current `https` URL you were given by running ngrok. Append with the path `/api/messages` used by this sample
    - Ensure that you've [enabled the Teams Channel](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0)
    - __*If you don't have an Azure account*__ you can use this [Bot Framework registration](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework)

- Update the `resources/application.properties` configuration for the bot to use the Microsoft App Id and App Password from the Bot Framework registration. (Note the App Password is referred to as the "client secret" in the azure portal and you can always create a new client secret anytime.) The Task Modules using pages require the deployed bot's path in BaseUrl.

- __*This step is specific to Teams.*__
    - **Edit** the `manifest.json` contained in the `teamsAppManifest` folder to replace your Microsoft App Id (that was created when you registered your bot earlier) *everywhere* you see the place holder string `<<YOUR-MICROSOFT-APP-ID>>` (depending on the scenario the Microsoft App Id may occur multiple times in the `manifest.json`). **Note:** the Task Modules containing pages will require the deployed bot's domain in validDomains of the manifest.
    - **Zip** up the contents of the `teamsAppManifest` folder to create a `manifest.zip`
    - **Upload** the `manifest.zip` to Teams (in the Apps view click "Upload a custom app")

- From the root of this project folder:
    - Build the sample using `mvn package`
    - Unless done previously, install the packages in the local cache by using `mvn install`
    - Run it by using `java -jar .\target\bot-teams-task-module-sample.jar`


## Interacting with the bot in Teams

> Note this `manifest.json` specified that the bot will be installed in "personal", "team" and "groupchat" scope which is why you immediately entered a one on one chat conversation with the bot. You can at mention the bot in a group chat or in a Channel in the Team you installed it in. Please refer to Teams documentation for more details.

You can interact with this bot by sending it a message. The bot will respond with a Hero Card with a button which will display a Task Module when clicked.  The Task Module demonstrates retrieving input from a user through a Text Block and a Submit button.

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
az deployment sub create --name "teamsTaskModuleDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="teamsTaskModulePlan" newWebAppName="teamsTaskModuleBot" groupLocation="westus" newAppServicePlanLocation="westus"
```

#### To an existing Resource Group
```
az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="teamsTaskModuleBot" newAppServicePlanName="teamsTaskModulePlan" appServicePlanLocation="westus" --name "teamsTaskModuleBot"
```

### 5. Update app id and password
In src/main/resources/application.properties update
- `MicrosoftAppPassword` with the botsecret value
- `MicrosoftAppId` with the appid from the first step

### 6. Deploy the code
- Execute `mvn clean package`
- Execute `mvn azure-webapp:deploy -Dgroupname="<groupname>" -Dbotname="<bot-app-service-name>"`

After the bot is deployed, you only need to execute #6 if you make changes to the bot.

## Further reading

- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://github.com/microsoft/azure-maven-plugins/tree/develop/azure-webapp-maven-plugin)
- [How Microsoft Teams bots work](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics-teams?view=azure-bot-service-4.0&tabs=javascript)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)

