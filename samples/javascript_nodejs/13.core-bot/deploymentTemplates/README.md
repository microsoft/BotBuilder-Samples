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

```bash
# Pass in the path to the ARM template for the --template-file argument.
# The argument for --template-file comes from the name of the templates located in the same folder as this README.
az deployment create --name "<name-of-deployment>" --template-file "template-with-new-rg.json" --subscription "<subscription-guid>" --location "westus" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" newServerFarmName="<name-of-server-farm>" newWebAppName="<name-of-web-app>" groupName="<new-group-name>" alwaysBuildOnDeploy=true
```

> *These instructions shows how to zipdeploy your code using `az webapp`. App Service's default behavior for zipdeploy is to not build the code or install any dependencies (e.g. you must zip up your binaries or your `node_modules` folder). If you do want the App service to build and/or install dependencies, add the optional parameter `"*
>
> *For more information on Kudu, visit their [GitHub repository][Kudu-Wiki].*

To see all available ARM template parameters and their descriptions, scroll down or click [here](#ARM-Template-Parameters-(new-Resource-Group)).

  [Kudu-Wiki]: https://github.com/projectkudu/kudu/wiki

#### 3. Retrieve or create necessary IIS/Kudu files via `az bot`

*For C# bots this command is necessary if you want Kudu to build on deployment (e.g. `alwaysBuildOnDeploy=true`):*
```bash
# For C# bots, it's necessary to provide the path to the .csproj file relative to --code-dir. This can be performed via the --proj-file-path argument
az bot prepare-deploy --lang Csharp --code-dir ".." --proj-file-path "./MyBot.csproj"
# The command would resolve --code-dir and --proj-file-path to "../MyBot.csproj"
```

*For Node.js bots:*
```bash
# This command will fetch a web.config which is needed for Node.js apps to work with IIS on Azure App Services
az bot prepare-deploy --code-dir ".." --lang Node
```

#### 4. Zip up the code directory manually
When deploying the ARM template, if the parameter `"alwaysBuildOnDeploy"` was set to `true` then you do not need to include your binaries or the `node_modules` folder in the zipped code as Kudu will build your code or install the NPM packages.

If it was set to `false` you must include the binaries and `node_modules` or the bot will not run when using zipdeploy. Note, the default value for this parameter is `false`.

#### 5. Deploy code to Azure using `az webapp`

```bash
az webapp deployment source config-zip --subscription "<subscription-guid>" --location "westus" --resource-group "<new-group-name>" --name "<name-of-web-app>" --src "Path/to/zipped/code.zip" 
# The --timeout argument is an optional and configurable timeout in seconds for checking the status of deployment.
```
___


### ARM Template Parameters (new Resource Group):
```json
"parameters": {
    "groupLocation": {
        "defaultValue": "westus",
        "type": "string",
        "metadata": {
            "description": "Specifies the location of the Resource Group. Defaults to \"westus\"."
        }
    },
    "groupName": {
        "type": "string",
        "metadata": {
            "description": "Specifies the name of the Resource Group."
        }
    },
    "appId": {
        "type": "string",
        "metadata": {
            "description": "Active Directory App ID, set as MicrosoftAppId in the Web App's Application Settings."
        }
    },
    "appSecret": {
        "type": "string",
        "metadata": {
            "description": "Active Directory App Password, set as MicrosoftAppPassword in the Web App's Application Settings. Defaults to \"\"."
        }
    },
    "botId": {
        "type": "string",
        "metadata": {
            "description": "The globally unique and immutable bot ID. Also used to configure the displayName of the bot, which is mutable."
        }
    },
    "botSku": {
        "defaultValue": "F0",
        "type": "string",
        "metadata": {
            "description": "The pricing tier of the Bot Service Registration. Acceptable values are F0 and S1."
        }
    },
    "newServerFarmName": {
        "type": "string",
        "metadata": {
            "description": "The name of the App Service Plan."
        }
    },
    "newServerFarmSku": {
        "type": "object",
        "defaultValue": {
            "name": "S1",
            "tier": "Standard",
            "size": "S1",
            "family": "S",
            "capacity": 1
        },
        "metadata": {
            "description": "The SKU of the App Service Plan. Defaults to Standard values."
        }
    },
    "newServerFarmLocation": {
        "type": "string",
        "defaultValue": "westus",
        "metadata": {
            "description": "The location of the App Service Plan. Defaults to \"westus\"."
        }
    },
    "newWebAppName": {
        "type": "string",
        "defaultValue": "",
        "metadata": {
            "description": "The globally unique name of the Web App. Defaults to the value passed in for \"botId\"."
        }
    },
    "alwaysBuildOnDeploy": {
        "type": "bool",
        "defaultValue": false,
        "metadata": {
            "description": "Configures environment variable SCM_DO_BUILD_DURING_DEPLOYMENT on Web App. When set to true, the Web App will automatically build or install NPM packages when a deployment occurs."
        }
    }
}
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
az group deployment create --name "<name-of-deployment>" --resource-group "<name-of-resource-group>" --template-file "template-with-preexisting-rg.json" --subscription "<subscription-guid>" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" newWebAppName="<name-of-web-app>" alwaysBuildOnDeploy=false newServerFarmName="<name-of-server-farm>"
```

##### Command with preexisting App Service Plan/Server Farm:
```bash
# The difference between this example command and the previous command is
# this command uses "existingServerFarm" as a parameter instead of "newServerFarmName"

# If both parameters are passed in, the ARM template provided will try to create a new Web App on the "existingServerFarm".
# Pass in the path to the ARM template for the --template-file argument.
# The argument for --template-file comes from the name of the templates located in the same folder as this README.

az group deployment create --name "<name-of-deployment>" --resource-group "<name-of-resource-group>" --template-file "template-with-preexisting-rg.json" --subscription "<subscription-guid>" --parameters appId="<msa-app-guid>" appSecret="<msa-app-password>" botId="<id-or-name-of-bot>" newWebAppName="<name-of-web-app>" alwaysBuildOnDeploy=false existingServerFarm="<name-of-server-farm>"
```

> *These instructions shows how to zipdeploy your code using `az webapp`. App Service's default behavior for zipdeploy is to not build the code or install any dependencies (e.g. you must zip up your binaries or your `node_modules` folder). If you do want the App service to build and/or install dependencies, add the optional parameter `"*
>
> *For more information on Kudu, visit their [GitHub repository][Kudu-Wiki].*

To see all available ARM template parameters and their descriptions, scroll down or click [here](#ARM-Template-Parameters-(preexisting-Resource-Group)).

  [Kudu-Wiki]: https://github.com/projectkudu/kudu/wiki

#### 3. Retrieve or create necessary IIS/Kudu files via `az bot`

*For C# bots this command is necessary if you want Kudu to build on deployment (e.g. `alwaysBuildOnDeploy=true`):*
```bash
# For C# bots, it's necessary to provide the path to the .csproj file relative to --code-dir. This can be performed via the --proj-file-path argument
az bot prepare-deploy --lang Csharp --code-dir ".." --proj-file-path "./MyBot.csproj"
# The command would resolve --code-dir and --proj-file-path to "../MyBot.csproj"
```

*For Node.js bots:*
```bash
# This command will fetch a web.config which is needed for Node.js apps to work with IIS on Azure App Services
az bot prepare-deploy --code-dir ".." --lang Node
```

#### 4. Zip up the code directory manually
When deploying the ARM template, if the parameter `"alwaysBuildOnDeploy"` was set to `true` then you do not need to include your binaries or the `node_modules` folder in the zipped code as Kudu will build your code or install the NPM packages.

If it was set to `false` you must include the binaries and `node_modules` or the bot will not run when using zipdeploy. Note, the default value for this parameter is `false`.

#### 5. Deploy code to Azure using `az webapp`

```bash
az webapp deployment source config-zip --subscription "<subscription-guid>" --location "westus" --resource-group "<new-group-name>" --name "<name-of-web-app>" --src "Path/to/zipped/code.zip" 
# The --timeout argument is an optional and configurable timeout in seconds for checking the status of deployment.
```
___


### ARM Template Parameters (preexisting Resource Group):
```json
"parameters": {
    "appId": {
        "type": "string",
        "metadata": {
            "description": "Active Directory App ID, set as MicrosoftAppId in the Web App's Application Settings."
        }
    },
    "appSecret": {
        "type": "string",
        "metadata": {
            "description": "Active Directory App Password, set as MicrosoftAppPassword in the Web App's Application Settings. Defaults to \"\"."
        }
    },
    "botId": {
        "type": "string",
        "metadata": {
            "description": "The globally unique and immutable bot ID. Also used to configure the displayName of the bot, which is mutable."
        }
    },
    "botSku": {
        "defaultValue": "F0",
        "type": "string",
        "metadata": {
            "description": "The pricing tier of the Bot Service Registration. Acceptable values are F0 and S1."
        }
    },
    "newServerFarmName": {
        "type": "string",
        "defaultValue": "",
        "metadata": {
            "description": "The name of the new App Service Plan."
        }
    },
    "newServerFarmSku": {
        "type": "object",
        "defaultValue": {
            "name": "S1",
            "tier": "Standard",
            "size": "S1",
            "family": "S",
            "capacity": 1
        },
        "metadata": {
            "description": "The SKU of the App Service Plan. Defaults to Standard values."
        }
    },
    "newServerFarmLocation": {
        "type": "string",
        "defaultValue": "westus",
        "metadata": {
            "description": "The location of the App Service Plan. Defaults to \"westus\"."
        }
    },
    "existingServerFarm": {
        "type": "string",
        "defaultValue": "",
        "metadata": {
            "description": "Name of the existing App Service Plan/Server Farm used to create the Web App for the bot."
        }
    },
    "newWebAppName": {
        "type": "string",
        "defaultValue": "",
        "metadata": {
            "description": "The globally unique name of the Web App. Defaults to the value passed in for \"botId\"."
        }
    },
    "alwaysBuildOnDeploy": {
        "type": "bool",
        "defaultValue": false,
        "metadata": {
            "comments": "Configures environment variable SCM_DO_BUILD_DURING_DEPLOYMENT on Web App. When set to true, the Web App will automatically build or install NPM packages when a deployment occurs."
        }
    }
}
```