# bot-authentication-sni

Bot Framework v4 echo bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back.

## To try this sample

- Install [NGrok](https://learn.microsoft.com/en-us/azure/bot-service/bot-service-debug-channel-ngrok?view=azure-bot-service-4.0)

- From a command prompt start ngrok
  ```
  ngrok http 3978 --host-header rewrite
  ```

- Record the Ngrok forwarding URL.  For example: "https://8078-68-227-112-63.ngrok-free.app"

- In a terminal, navigate to `botbuilder-samples\samples\python\85.bot-authentication-sni` folder

- Activate your desired virtual environment

- In the terminal, type `pip install -r requirements.txt`

- Create an SSL/TLS certificate using KeyVault
  1. Create a KeyVault resource and assign _the KeyVault Administrator_ role to have permission to create a new certificate.

  2. Under the Certificates section, hit on Generate/Import, complete the form, and create the certificate in PKCS format.
     - This sample assume a OneCert domain has been onboarded at setup as a provider in KeyVault
     - The Subject name is:  "CN={your-onecert-domain}"

- Create Azure App and Bot
  1. Create App Registration
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
  2. Create an Azure Bot in the desired resource group. Use the App Registration from the previous step.
     - Set the messaging enpoint to: {ngrok-forwarding-url}/api/messages

- Set config.py variables
  - MicrosoftAppType: {SingTenant | MultiTenant}
  - MicrosoftAppId: {appId}
  - MicrosoftAppTenantId: {tenantId-if-single-tenant}
  - MicrosoftAppKeyVaultName: Name of the KeyVault containing the certificate.
  - MicrosoftAppCertificateName: Name of the certificate in the KeyVault.
  - MicrosoftAppCertificateThumbprint: Thumbprint of the certificate

- Run your bot with `python app.py`

## Interacting with the bot

Use "Test in WebChat" from the Azure Bot created above.

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
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
