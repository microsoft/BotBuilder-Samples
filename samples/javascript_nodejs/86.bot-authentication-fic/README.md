# Authentication Bot using Federation Identity Certificate (FIC)

Bot Framework v4 bot authentication using Federation Identity Certificate (FIC). Currently, FIC feature is supported for Microsoft tenants only.

This bot has been created using [Bot Framework](https://dev.botframework.com/), is shows how to use the bot authencation capabilities of Azure Bot Service. In this sample, we use federated identity certificate configuration to create the Bot Framework Authentication.

## Prerequisites

- [Node.js](https://nodejs.org) version 18 or higher

  ```bash
  # determine node version
  node  --version
  ```

- [Bot Framework SDK](https://github.com/microsoft/botbuilder-dotnet/releases) version 4.23.1 onwards

## To try this sample

- In a terminal, navigate to `samples/javascript_nodejs/86.bot-authentication-fic`

  ```bash
  cd  samples/javascript_nodejs/86.bot-authentication-fic
  ```

- Install modules

  ```bash
  npm  install
  ```

- Create an user assigned managed identity.

  - Record the client ID of the managed identity and add the same to .env file.

- Create Azure App and Bot

  - Create App Registration
    - This can be either Single or Multi tenant.
    - Record the Application ID.
    - Add the Federation Identity Certificate (FIC) to the App Registration.
      - To create trust using the FIC, we need to link the managed identity to the App Registration.
        - Click on the add credential under Federated Credential
        - On the Add a credential page, select the Federated credential scenario as "Customer Managed Keys".
        - Select the managed identity that you created in the previous step.
        - Enter name for the credential and click on Add.
  - Create an Azure Bot in the desired resource group. Use the App Registration from the previous step.

  - Add the User managed identity created in previous step to the Azure App Service under Configuration -> Identity -> User Assigned Managed Identity.

- Set .env variables

  - MicrosoftAppType: {SingTenant | MultiTenant}

  - MicrosoftAppId: {appId}

  - MicrosoftAppTenantId: {tenantId}

  - MicrosoftAppClientId: {clientId of managed identity}

- Start the bot

  ```bash
  npm  start
  ```

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
