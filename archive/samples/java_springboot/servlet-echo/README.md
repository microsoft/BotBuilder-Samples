# Servlet EchoBot

This demonstrates how to create a Bot using the Bot Framework 4 SDK Preview for Java in Azure.

This sample is a Java Servlet app and uses the Azure CLI and azure-webapp Maven plugin to deploy.

## Prerequisites

- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com)
 
## Deploy the bot to Azure

As described on [Deploy your bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli), you will perform the first 4 steps to setup the Azure app, then deploy the code using the azure-webapp Maven plugin.

### 1. Login to Azure
From a command (or Powershell) prompt in the root of the bot folder, execute:  
`az login`
  
### 2. Set the subscription
`az account set --subscription "<azure-subscription>"`

If you aren't sure which subscription to use for deploying the bot, you can view the list of subscriptions for your account by using `az account list` command. 

### 3. Create an App registration
`az ad app create --display-name "<botname>" --password "<appsecret>" --available-to-other-tenants`

Replace `<botname>` and `<appsecret>` with your own values.

`<botname>` is the unique name of your bot.  
`<appsecret>` is a minimum 16 character password for you bot. 

Record the `appid` from the returned JSON

### 4. Create the Azure resources
Replace the values for `<appid>`, `<appsecret>`, `<botname>`, and `<groupname>` in the following commands:

#### To a new Resource Group
`az deployment sub create --name "echoBotDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="echoBotPlan" newWebAppName="echoBot" groupLocation="westus" newAppServicePlanLocation="westus"`

#### To an existing Resource Group
`az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="echoBot" newAppServicePlanName="echoBotPlan" appServicePlanLocation="westus" --name "echoBot"`

### 5. Update app id and password
In src/main/resources/application.properties update 
  - `MicrosoftAppPassword` with the appsecret value
  - `MicrosoftAppId` with the appid from the first step

### 6. Deploy the code
- Execute `mvn clean package` 
- Execute `mvn azure-webapp:deploy -Dgroupname="<groupname>" -Dbotname="<botname>"`

If the deployment is successful, you will be able to test it via "Test in Web Chat" from the Azure Portal using the "Bot Channel Registration" for the bot.

After the bot is deployed, you only need to execute #6 if you make changes to the bot.


## Reference

[Maven Plugin for Azure App Service](https://docs.microsoft.com/en-us/java/api/overview/azure/maven/azure-webapp-maven-plugin/readme?view=azure-java-stable)

[Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)

