# Authentication Bot using SSL/TLS certificates

Bot Framework v4 bot authentication using Certificate

This bot has been created using [Bot Framework](https://dev.botframework.com/), is shows how to use the bot authentication capabilities of Azure Bot Service. In this sample, we use a local or KeyVault certificate to create the Bot Framework Authentication.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) version 8.0

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Set app settings variables

  - MicrosoftAppType: Type of the App.

  - MicrosoftAppId: App Id of your bot.

  - MicrosoftAppTenantId: Tenant Id to which your bot belongs.

  - KeyVaultName: Name of the KeyVault containing the certificate.

  - CertificateName: Name of the certificate in the KeyVault.


- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/84.bot-authentication-certificate`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/84.bot-authentication-certificate` folder
  - Select `AuthCertificateBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Interacting with the bot

This sample uses the bot authentication capabilities of Azure Bot Service, providing features to make it easier to develop a bot that authenticates users using digital security certificates. You just need to provide the certificate data linked to the managed identity and run the bot, then communicate with it to validate its correct authentication.

## SSL/TLS certificate

An SSL/TLS certificate is a digital object that allows systems to verify identity and subsequently establish an encrypted network connection with another system using the Secure Sockets Layer/Transport Layer Security (SSL/TLS) protocol. Certificates are issued using a cryptographic system known as public key infrastructure (PKI). PKI allows one party to establish the identity of another through the use of certificates if they both trust a third party, known as a certificate authority. SSL/TLS certificates therefore function as digital identity documents that protect network communications and establish the identity of websites on the Internet as well as resources on private networks.

## How to create an SSL/TLS certificate

There are two possible options to create SSL/TSL certificate. Below is a step-by-step description of each one:

### Using local environment

1. Run the following command in a local PowerShell

```
$cert = New-SelfSignedCertificate -CertStoreLocation "<directory-to-store-certificate>" -Subject "CN=<certificate-name>" -KeySpec KeyExchange
```

1. Then, type _Manage User Certificates_ in the Windows search bar and hit enter

2. The certificate will be located in the _user certificates_ folder, under _personal_ directory.

3. Export the certificate to _pfx_ format including the key(The default location is _system32_ folder).

4. Go to the certificate location and run the following command to generate a _pem_ file:

```
OpenSSL pkcs12 -in <certificate-name>.pfx -out c:\<certificate-name>.pem –nodes
```

5. Upload the generated certificate to the Azure app registration.

### Using KeyVault

1. Create a KeyVault resource and assign _the KeyVault Administrator_ role to have permission to create a new certificate.

2. Under the Certificates section, hit on Generate/Import, complete the form, and create the certificate in PEM format.

3. Go to the details of the certificate that you created and enable it.

4. Download the certificate in CER format and then upload it to the Azure app registration.

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
