# Authentication Bot using Subject Name/Issuer

Bot Framework v4 bot authentication using Subject Name/Issuer

This bot has been created using [Bot Framework](https://dev.botframework.com/), is shows how to use the bot authentication capabilities of Azure Bot Service. In this sample, we use a local or KeyVault certificate and the MSAL Subject Name/Issuer configuration to create the Bot Framework Authentication.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) version 6.0

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Open from Visual Studio
  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/85.bot-authentication-sni` folder
  - Select `AuthSNIBot.csproj` file

- Create an SSL/TLS certificate using KeyVault
  1. Create a KeyVault resource and assign _the KeyVault Administrator_ role to have permission to create a new certificate.

  2. Under the Certificates section, hit on Generate/Import, complete the form, and create the certificate in PEM format.

  3. Go to the details of the certificate that you created and enable it and record the subject name

- Create Azure App and Bot
  - Create App Registration
    - This can be either Single or Multi tenant
    - Record the Application ID
    - Add this to the Manifest
      "trustedCertificateSubjects": [
          {
              "authorityId": "00000000-0000-0000-0000-000000000001",
  
              "subjectName": "certificate_subject_name",
 
              "revokedCertificateIdentifiers": []
          }
      ]
  - Create an Azure Bot in the desired resource group. Use the App Registration from the previous step.

- Set appsettings.json variables

  - MicrosoftAppType: {SingTenant | MultiTenant}

  - MicrosoftAppId: {appId}

  - MicrosoftAppTenantId: {tenantId}

  - KeyVaultName: Name of the KeyVault containing the certificate.

  - CertificateName: Name of the certificate in the KeyVault.

- Run the bot from Visual Studio:

## Interacting with the bot

This sample uses the bot authentication capabilities of Azure Bot Service, providing features to make it easier to develop a bot that authenticates users using digital security certificates. You just need to provide the certificate data linked to the managed identity and run the bot, then communicate with it to validate its correct authentication.

## SSL/TLS certificate

An SSL/TLS certificate is a digital object that allows systems to verify identity and subsequently establish an encrypted network connection with another system using the Secure Sockets Layer/Transport Layer Security (SSL/TLS) protocol. Certificates are issued using a cryptographic system known as public key infrastructure (PKI). PKI allows one party to establish the identity of another through the use of certificates if they both trust a third party, known as a certificate authority. SSL/TLS certificates therefore function as digital identity documents that protect network communications and establish the identity of websites on the Internet as well as resources on private networks.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Azure Portal](https://portal.azure.com)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [SSL/TLS certificates](https://www.digicert.com/tls-ssl/tls-ssl-certificates)
