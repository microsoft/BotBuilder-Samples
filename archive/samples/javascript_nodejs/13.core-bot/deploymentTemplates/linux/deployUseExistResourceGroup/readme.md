# Usage
BotApp must be deployed prior to AzureBot.

### Command line:
`az login`<br>
`az deployment group create --resource-group <group-name> --template-file <template-file> --parameters @<parameters-file>`

## Parameters for template-BotApp-with-rg.json:

- **appServiceName**: (required)        The Name of the Bot App Service.
- (Pick an existing App Service Plan or create a new App Service Plan.)
  - **existingAppServicePlanName**:     The name of the App Service Plan.
  - **existingAppServicePlanLocation**: The location of the App Service Plan.
  - **newAppServicePlanName**:          The name of the App Service Plan.
  - **newAppServicePlanLocation**:      The location of the App Service Plan.
  - **newAppServicePlanSku**:           The SKU of the App Service Plan. Defaults to Standard values.
- **appId**: (required)                 Active Directory App ID or User-Assigned Managed Identity Client ID, set as MicrosoftAppId in the Web App's Application Settings.
- **appSecret**: (required for MultiTenant and SingleTenant)  Active Directory App Password, set as MicrosoftAppPassword in the Web App's Application Settings.

## Parameters for template-AzureBot-with-rg.json:

- **azureBotId**: (required)            The globally unique and immutable bot ID.
- **azureBotSku**:                      The pricing tier of the Bot Service Registration. Allowed values are: F0, S1(default).
- **azureBotRegion**:                   Specifies the location of the new AzureBot. Allowed values are: global(default), westeurope.
- **botEndpoint**:                      Use to handle client messages, Such as `https://<botappServiceName>.azurewebsites.net/api/messages`.
- **appId**: (required)                 Active Directory App ID or User-Assigned Managed Identity Client ID, set as MicrosoftAppId in the Web App's Application Settings.