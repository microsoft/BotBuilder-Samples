﻿# Bot using Federation Identity Credentials

Bot Framework v4 bot authentication using Federation Identity Credentials (FIC). Currently, FIC feature is supported single tenant bots only.

This bot has been created using [Bot Framework](https://dev.botframework.com/), is shows how to use the bot authentication capabilities of Azure Bot Service. In this sample, we use federated identity certificate configuration to create the Bot Framework Authentication.

This bot uses `FederatedServiceClientCredentialsFactory` which is registered in Startup.cs.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) version 8.0

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- [Bot Framework SDK](https://github.com/microsoft/botbuilder-dotnet/releases) version 4.22.8 onwards


## To try this sample

- Create an user assigned managed identity.
  - Record the client ID of the managed identity.

- For a new bot
  - Create Azure App and Bot
    - Create App Registration
      - This can be either Single or Multi tenant.
      - Record the Application and Tenant ID's.
      - To create trust using the FIC, we need to link the managed identity to the App Registration.  On the App Registration:
        - Click on the add credential under **Certificates & Secrets**, **Federated credentials**
        - On the Add a credential page, select the Federated credential scenario as **Customer Managed Keys**.
        - Select the managed identity that you created in the previous step.
        - Enter name for the credential and click on Add.
        
    - Create an **Azure Bot** in the desired resource group and use the App Registration from the previous step.

- For an existing bot
   - Navigate to the **App Registration** for the **Azure Bot**
     - To create trust using the FIC, we need to link the managed identity to the App Registration.  On the App Registration:
      - Click on the add credential under **Certificates & Secrets**, **Federated credentials**
      - On the Add a credential page, select the Federated credential scenario as **Customer Managed Keys**.
      - Select the managed identity that you created in the previous step.
      - Enter name for the credential and click on Add.
   - Navigate to the **App Service** for the bot 
     - Add the User managed identity created in previous step to the Azure App Service under Configuration -> Identity -> User Assigned Managed Identity.
   
- Set appsettings.json variables

  - MicrosoftAppType: {SingTenant | MultiTenant}

  - MicrosoftAppId: {appId}

  - MicrosoftAppTenantId: {tenantId}

  - MicrosoftAppClientId: {clientId of managed identity}

- A bot using Federated Credentials, like UserManagedIdentity, cannot be run locally.  It must be deployed to Azure.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
