# Bot Authentication

Bot Framework v4 bot authentication sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use authentication in your bot using OAuth.

The sample uses the bot authentication capabilities in [Azure Bot Service](https://docs.botframework.com), providing features to make it easier to develop a bot that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, etc.

NOTE: Microsoft Teams currently differs slightly in the way auth is integrated with the bot. Refer to sample 6 [here](https://github.com/OfficeDev/Microsoft-Teams-Samples#bots-samples-using-the-v4-sdk).

NOTE: This sample has been updated to use Federated Identity Credentials (FIC), but it also supports ClientSecret authentication following this guide: [Add Authentication to your bot via Azure Bot Service](https://learn.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=userassigned%2Caadv2%2Cjavascript).

## Prerequisites

- [Node.js](https://nodejs.org) version 18 or higher

    ```bash
    # determine node version
    node --version
    ```
- [Bot Framework SDK](https://github.com/microsoft/botbuilder-dotnet/releases) version 4.23.1 onwards

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a console, navigate to `samples/javascript_nodejs/18.bot-authentication`

    ```bash
    cd samples/javascript_nodejs/18.bot-authentication
    ```

- Install modules

    ```bash
    npm install
    ```

- Create a MS Entra ID application to register a bot resource in Azure and a separate MS Entra ID application to function as the identity provider.

  - Create a User Assigned Managed Identity
      - Record the client ID of the managed identity.
  - Create an App Service
      - Add the User managed identity created in previous step to the Azure App Service under Configuration -> Identity -> User Assigned Managed Identity.
      - Record the **Default domain** on the **Overview** tab.
  - Create App Registration
      - This should be Single tenant (Currently, FIC feature is supported in single tenant bots only).
      - Record the Application and Tenant ID's.
      - To create trust using the FIC, we need to link the managed identity to the App Registration. On the App Registration:
        - Click on the add credential under **Certificates & Secrets**, **Federated credentials**.
        - On the Add a credential page, select the Federated credential scenario as **Managed Identity**.
        - Select the managed identity that you created in the previous step.
        - Enter name for the credential and click on **Add**.
  - Create an **Azure Bot** in the desired resource group and use the App Registration from the previous step
     - Update the **Messaging endpoint** on the **Configuration** tab to:  https://{default-domain}/api/messages.
  - Create the **Identity Provider** 
     - Create the AppRegistration 
        - Select SingleTenant and for the Redirect URL, use one of the followings: [OAuth URL support in Azure AI Bot Service](https://learn.microsoft.com/en-us/azure/bot-service/ref-oauth-redirect-urls?view=azure-bot-service-4.0).
        - Record the Application and Tenant ID's.
        - Click on the add credential under **Certificates & Secrets**, **Federated credentials**.
        - On the Add a credential page, select the Federated credential scenario as **Other Issuer**.
        - Provide information under **Connect your account** section:
           - Issuer: "https://login.microsoftonline.com/{tenant-ID}/v2.0"
           - Subject Identifier: "/eid1/c/pub/t/{base64-encoded-tenant-ID}/a/{base64-encoded-first-party-app-client-ID}/{unique-identifier-for-projected-identity}"
           - Audience: api://AzureADTokenExchange
           - Unique-identifier-for-projected-identity: Set a unique identifier

             NOTE: You can encode the tenant ID using [this code](https://dotnetfiddle.net/p11CFZ), and for the encoded first-party-app-client-ID, you can use one of these values: `9ExAW52n_ky4ZiS_jhpJIQ`, `ND1y8_Vv60yhSNmdzSUR_A`.

        - In the **API Permisions** tab, grant the following permisions: openid, profile, Mail.Read, Mail.Send, User.Read, User.ReadBasic.All.
    - Register the Identity Provider with the bot
        - Add an **OAuth Connection Settings** in your **Azure Bot** resource, on the **Configuration** tab.
            - Service Provider: AAD v2 with Federated Credentials
            - Client ID: The App Registration ID of the identity provider created in the previous step
            - Unique Identifier: The unique identifier of the identity provider created in the previous step
            - Token Exchange URL: leave it blank
            - Login URL: leave it blank
            - Tenant ID: The Tenant ID of the identity provider created in the previous step
            - Scopes: The names of the permissions from the application registration
        - Test the connection and concent the requested permissions.

- Set .env variables

  - MicrosoftAppType: SingleTenant

  - MicrosoftAppId: {appId}

  - MicrosoftAppTenantId: {tenantId}

  - MicrosoftAppClientId: {clientId of managed identity}

  - connectionName: {Configured bot's OAuth connection name.}

- A bot using Federated Credentials, like UserManagedIdentity, cannot be run locally. It must be deployed to Azure.

## Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Interacting with the bot

This sample uses bot authentication capabilities in Azure Bot Service, providing features to make it easier to develop a bot that authenticates users to various identity providers such as Azure AD (Azure Active Directory), GitHub, Uber, etc. These updates also take steps towards an improved user experience by eliminating the magic code verification for some clients.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Azure Portal](https://portal.azure.com)
- [Add Authentication to Your Bot Via Azure Bot Service](https://learn.microsoft.com/en-us/azure/bot-service/bot-builder-authentication?view=azure-bot-service-4.0&tabs=userassigned%2Caadv2%2Cjavascript)
- [Implement Authentication with Federated Identity Credentials](https://learn.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-federated-credential)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
