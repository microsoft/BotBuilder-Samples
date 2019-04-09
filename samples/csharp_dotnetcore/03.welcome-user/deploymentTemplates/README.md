# Bot Framework ARM Templates:

This README contains information on how to deploy your local bot to Azure.

## Table of Contents:

- [Prerequisites](#Prerequisites)
- [Deploying via ARM template (with new Resource Group)](#deploying-via-arm-template-with-new-resource-group)
- [Deploying via ARM template (with preexisting Resource Group)](#deploying-via-arm-template-with-preexisting-resource-group)

___

## [Prerequisites](#Table-of-Contents):

#### Install Azure CLI >=2.0.61:
Visit the following page to find the correct installer for your OS:
- https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest

___

## [Deploying via ARM template (with new Resource Group)](#Table-of-Contents)

We recommend provisioning Azure resources through ARM templates via the [Azure CLI][ARM-CLI]. It is also possible to deploy ARM templates via the [Azure Portal][ARM-Portal], [PowerShell][ARM-PowerShell] and the [REST API][ARM-REST].

To install the latest version of the Azure CLI visit [this page][Install-CLI].

  [ARM-CLI]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli
  [ARM-Portal]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-portal
  [ARM-PowerShell]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy
  [ARM-REST]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-rest
  [Install-CLI]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest

___

#### 1. Create an App registration
Your bot requires a Registered app that provides the bot access to the bot framework service for sending and receiving authenticated messages. 

To create an App registration via the Azure CLI, perform the following command:
```bash
# Replace "displayName" and "AtLeastSixteenCharacters_0" with your specified values.
# The --password argument must be at least 16 characters in length, and have at least 1 lowercase char, 1 uppercase char, 1 numerical char, and 1 special char (e.g. !?-_+=)
az ad app create --display-name "displayName" --password "AtLeastSixteenCharacters_0" --available-to-other-tenants
```

This command will output JSON with the key "appId", save the value of this key for the ARM deployment, where it will be used for the `"appId"` parameter. The password provided will be used for the `"appSecret"` parameter.

> *It is also possible to create and manage Registered Apps via the [Azure portal][Preview-Portal]. Be sure to also create a password when creating the application.*

  [Preview-Portal]: https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade

#### 2. Create a resource group and the Azure resources
Next, you'll use the ARM template to create the resources specified in it. In this case, we are provding App Service Plan, Web App, and Bot Channels Registration. 

> *Note: The `botId` parameter should be globally unique and is used as the immutable bot ID. Also used to configure the displayName of the bot, which is mutable.*

```bash
# Pass in the path to the ARM template for the --template-file argument.
# The argument for --template-file comes from the name of the templates located in the same folder as this README.
az deployment create --name "<name-of-deployment>" --template-file "template-with-new-rg.json" --subscription "<subscription-guid>" --location "westus" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" botSku=F0 newAppServicePlanName="<name-of-app-service-plan>" newWebAppName="<name-of-web-app>" groupName="<new-group-name>" groupLocation="westus" newAppServicePlanLocation="westus"
```

> *These instructions shows how to zipdeploy your code using `az webapp`. App Service's default behavior for zipdeploy is to not build the code or install any dependencies (e.g. you must zip up your binaries or your `node_modules` folder).*
>
> *For more information on Kudu, visit their [GitHub repository][Kudu-Wiki].*

To see all available ARM template parameters and their descriptions, scroll down or click [here](#ARM-Template-Parameters-(new-Resource-Group)).

  [Kudu-Wiki]: https://github.com/projectkudu/kudu/wiki

#### 3. Retrieve or create necessary IIS/Kudu files via `az bot`

*For C# bots this command is necessary if Kudu is configured to build on zip deployment (i.e. [SCM_DO_BUILD_DURING_DEPLOYMENT=true][Enable/Disable Build]):*
```bash
# For C# bots, it's necessary to provide the path to the .csproj file relative to --code-dir. This can be performed via the --proj-file-path argument
az bot prepare-deploy --lang Csharp --code-dir "." --proj-file-path "MyBot.csproj"
# The command would resolve --code-dir and --proj-file-path to "./MyBot.csproj"
```

  [Enable/Disable Build]: https://github.com/projectkudu/kudu/wiki/Configurable-settings#enabledisable-build-actions

*For Node.js bots:*
```bash
# This command will fetch a web.config which is needed for Node.js apps to work with IIS on Azure App Services
az bot prepare-deploy --code-dir "." --lang Node
```

#### 4. Zip up the code directory manually

When using the non-configured [`zipdeploy` API][Kudu-Zipdeploy] to deploy your bot's code, Web App/Kudu's behavior is as follows:

> _**Kudu assumes by default that deployments from zip files are ready to run and do not require additional build steps during deployment**, such as `npm install` or `dotnet restore`/`dotnet publish`._

  [Kudu-Zipdeploy]: https://github.com/projectkudu/kudu/wiki/Deploying-from-a-zip-file-or-url

As such, it is important to include your built code and with all necessary dependencies in the zip file being deployed to the Web App, otherwise your bot will not work as intended.

#### 5. Deploy code to Azure using `az webapp`
At this point we are ready to deploy the code to the Azure Web App. 
```bash
az webapp deployment source config-zip --subscription "<subscription-guid>" --resource-group "<new-group-name>" --name "<name-of-web-app>" --src "Path/to/zipped/code.zip" 
# The --timeout argument is an optional and configurable timeout in seconds for checking the status of deployment.
```
___

## [Deploying via ARM template (with preexisting Resource Group)](#Table-of-Contents)

We recommend provisioning Azure resources through ARM templates via the [Azure CLI][ARM-CLI]. It is also possible to deploy ARM templates via the [Azure Portal][ARM-Portal], [PowerShell][ARM-PowerShell] and the [REST API][ARM-REST].

To install the latest version of the Azure CLI visit [this page][Install-CLI].

  [ARM-CLI]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli
  [ARM-Portal]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-portal
  [ARM-PowerShell]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy
  [ARM-REST]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-rest
  [Install-CLI]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest

___

#### 1. Create an App registration
To create an App registration via the Azure CLI, perform the following command:
```bash
# Replace "displayName" and "AtLeastSixteenCharacters_0" with your specified values.
# The --password argument must be at least 16 characters in length, and have at least 1 lowercase char, 1 uppercase char, 1 special char, and 1 special char (e.g. !?-_+=)
az ad app create --display-name "displayName" --password "AtLeastSixteenCharacters_0" --available-to-other-tenants
```

This command will output JSON with the key "appId", save the value of this key for the ARM deployment, where it will be used for the `"appId"` parameter. The password provided will be used for the `"appSecret"` parameter.

> *It is also possible to create App registrations via [apps.dev.microsoft.com][Apps-List] or via the [Azure portal][Preview-Portal]. Be sure to also create a password when creating the application.*

  [Apps-List]: https://apps.dev.microsoft.com/#/appList
  [Preview-Portal]: https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade

#### 2. Create a resource group and the Azure resources

> *Note: The `botId` parameter should be globally unique and is used as the immutable bot ID. Also used to configure the displayName of the bot, which is mutable.*

##### Command with new App Service Plan/Server Farm:
```bash
# Pass in the path to the ARM template for the --template-file argument.
# The argument for --template-file comes from the name of the templates located in the same folder as this README.
az group deployment create --name "<name-of-deployment>" --resource-group "<name-of-resource-group>" --template-file "template-with-preexisting-rg.json" --subscription "<subscription-guid>" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" newWebAppName="<name-of-web-app>" newServerFarmName="<name-of-server-farm>"
```

##### Command with preexisting App Service Plan/Server Farm:
```bash
# The difference between this example command and the previous command is
# this command uses "existingServerFarm" as a parameter instead of "newServerFarmName"

# If both parameters are passed in, the ARM template provided will try to create a new Web App on the "existingServerFarm".
# Pass in the path to the ARM template for the --template-file argument.
# The argument for --template-file comes from the name of the templates located in the same folder as this README.

az group deployment create --name "<name-of-deployment>" --resource-group "<name-of-resource-group>" --template-file "template-with-preexisting-rg.json" --subscription "<subscription-guid>" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" newWebAppName="<name-of-web-app>" existingServerFarm="<name-of-server-farm>"
```

> *These instructions shows how to zipdeploy your code using `az webapp`. App Service's default behavior for zipdeploy is to not build the code or install any dependencies (e.g. you must zip up your binaries or your `node_modules` folder).*
>
> *For more information on Kudu, visit their [GitHub repository][Kudu-Wiki].*

To see all available ARM template parameters and their descriptions, scroll down or click [here](#ARM-Template-Parameters-(new-Resource-Group)).

  [Kudu-Wiki]: https://github.com/projectkudu/kudu/wiki

#### 3. Retrieve or create necessary IIS/Kudu files via `az bot`

*For C# bots this command is necessary if Kudu is configured to build on zip deployment (i.e. [SCM_DO_BUILD_DURING_DEPLOYMENT=true][Enable/Disable Build]):*
```bash
# For C# bots, it's necessary to provide the path to the .csproj file relative to --code-dir. This can be performed via the --proj-file-path argument
az bot prepare-deploy --lang Csharp --code-dir "." --proj-file-path "MyBot.csproj"
# The command would resolve --code-dir and --proj-file-path to "./MyBot.csproj"
```

  [Enable/Disable Build]: https://github.com/projectkudu/kudu/wiki/Configurable-settings#enabledisable-build-actions

*For Node.js bots:*
```bash
# This command will fetch a web.config which is needed for Node.js apps to work with IIS on Azure App Services
az bot prepare-deploy --code-dir "." --lang Node
```

#### 4. Zip up the code directory manually

When using the non-configured [`zipdeploy` API][Kudu-Zipdeploy] to deploy your bot's code, Web App/Kudu's behavior is as follows:

> _**Kudu assumes by default that deployments from zip files are ready to run and do not require additional build steps during deployment**, such as `npm install` or `dotnet restore`/`dotnet publish`._

  [Kudu-Zipdeploy]: https://github.com/projectkudu/kudu/wiki/Deploying-from-a-zip-file-or-url

As such, it is important to include your built code and with all necessary dependencies in the zip file being deployed to the Web App, otherwise your bot will not work as intended.

#### 5. Deploy code to Azure using `az webapp`
At this point we are ready to deploy the code to the Azure Web App. 
```bash
az webapp deployment source config-zip --subscription "<subscription-guid>" --resource-group "<new-group-name>" --name "<name-of-web-app>" --src "Path/to/zipped/code.zip" 
# The --timeout argument is an optional and configurable timeout in seconds for checking the status of deployment.
```