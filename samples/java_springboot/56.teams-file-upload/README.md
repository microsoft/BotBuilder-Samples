# Teams File Upload Bot

Bot Framework v4 file upload bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to
upload files to Teams from a bot and how to receive a file sent to a bot as an attachment.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites
- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.
- Microsoft Teams is installed and you have an account
- [ngrok](https://ngrok.com/) or equivalent tunnelling solution

## To try this sample

> Note these instructions are for running the sample on your local machine, the tunnelling solution is required because
the Teams service needs to call into the bot.

- Run ngrok - point to port 3978
- Create [Bot Framework registration resource](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart-registration) in Azure
    - Use the current `https` URL you were given by running ngrok. Append with the path `/api/messages` used by this sample
    - Ensure that you've [enabled the Teams Channel](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0)
    - __*If you don't have an Azure account*__ you can use this [Bot Framework registration](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework)
- Update the `resources/application.properties` configuration for the bot to use the Microsoft App Id and App Password from the Bot Framework registration. (Note the App Password is referred to as the "client secret" in the azure portal and you can always create a new client secret anytime.)
- __*This step is specific to Teams.*__
    - **Edit** the `manifest.json` contained in the  `teamsAppManifest` folder to replace your Microsoft App Id (that was created when you registered your bot earlier) *everywhere* you see the place holder string `<<YOUR-MICROSOFT-APP-ID>>` (depending on the scenario the Microsoft App Id may occur multiple times in the `manifest.json`)
    - **Zip** up the contents of the `teamsAppManifest` folder to create a `manifest.zip`
    - **Upload** the `manifest.zip` to Teams (in the Apps view click "Upload a custom app")
- From the root of this project folder:
    - Build the sample using `mvn package`
    - Unless done previously, install the packages in the local cache by using `mvn install`
    - Run it by using `java -jar .\target\bot-teams-file-upload-sample.jar`


## Interacting with the bot in Teams

> Note this `manifest.json` specified that the bot will be installed in "personal" scope which is why you immediately entered a one on one chat conversation with the bot. Please refer to Teams documentation for more details.

1. Sending a message to the bot will cause it to respond with a card that will prompt you to upload a file. The file that's being uploaded is the `teams-logo.png` in the `Files` directory in this sample. The `Accept` and `Decline` events illustrated in this sample are specific to Teams. You can message the bot again to receive another prompt.

1. You can send a file to the bot as an attachment in the message compose section in Teams. This will be delivered to the bot as a Message Activity and the code in this sample fetches and saves the file.

1. You can also send an inline image in the message compose section. This will be present in the attachments of the Activity and requires the Bot's access token to fetch the image.

## Deploy the bot to Azure

As described on [Deploy your bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli), you will perform the first 4 steps to setup the Azure app, then deploy the code using the azure-webapp Maven plugin.

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
az deployment sub create --name "teamsFileUploadDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="teamsFileUploadPlan" newWebAppName="teamsFileUpload" groupLocation="westus" newAppServicePlanLocation="westus"
```

#### To an existing Resource Group

```
az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="teamsFileUpload" newAppServicePlanName="teamsFileUploadPlan" appServicePlanLocation="westus" --name "teamsFileUpload"
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

## Further reading
- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://github.com/microsoft/azure-maven-plugins/tree/develop/azure-webapp-maven-plugin)
- [How Microsoft Teams bots work](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics-teams?view=azure-bot-service-4.0&tabs=javascript)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)

