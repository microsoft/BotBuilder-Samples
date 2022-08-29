Need deploy BotAppService before AzureBot
---
az login
az deployment sub create --template-file <template-file> --location <bot-region> --parameters @<parameters-file>
---

# parameters-for-template-BotApp-new-rg:

**groupName**:(required)                Specifies the name of the new Resource Group.
**groupLocation**:(required)            Specifies the location of the new Resource Group.

**appServiceName**:(required)           The location of the App Service Plan.
**appServicePlanName**:(required)       The name of the App Service Plan.
**appServicePlanLocation**:             The location of the App Service Plan. Defaults to use groupLocation.
**appServicePlanSku**:                  The SKU of the App Service Plan. Defaults to Standard values.

**appId**:(required)                                        Active Directory App ID or User-Assigned Managed Identity Client ID, set as MicrosoftAppId in the Web App's Application Settings.
**appSecret**:(required for MultiTenant and SingleTenant)   Active Directory App Password, set as MicrosoftAppPassword in the Web App's Application Settings.


# parameters-for-template-AzureBot-new-rg:

**groupName**:(required)            Specifies the name of the new Resource Group.
**groupLocation**:(required)        Specifies the location of the new Resource Group.

**azureBotId**:(required)           The globally unique and immutable bot ID. Also used to configure the displayName of the bot, which is mutable.
**azureBotSku**:                    The pricing tier of the Bot Service Registration. **Allowed values are: F0, S1(default)**.
**azureBotRegion**:                 Specifies the location of the new AzureBot. **Allowed values are: global(default), westeurope**.
**botEndpoint**:                    Use to handle client messages, Such as https://<botappServiceName>.azurewebsites.net/api/messages.

**appId**:(required)                                        Active Directory App ID or User-Assigned Managed Identity Client ID, set as MicrosoftAppId in the Web App's Application Settings.