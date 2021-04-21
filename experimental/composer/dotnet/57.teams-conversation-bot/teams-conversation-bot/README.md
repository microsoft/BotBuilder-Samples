## Bot Project

Bot project is the launcher project for the bots written in declarative form (JSON), using the Runtime, for the Bot Framework SDK.

### CI/CD Deployment

You can deploy your bot to an Azure Web App with the following script from an Azure pipeline or GitHub workflow using an [Azure CLI task](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/azure-cli).

```bash
./scripts/deploy.ps1 -name my-bot -environment prod -luisAuthoringKey XXXXXXXXX -luisAuthoringRegion westeurope
```

The Azure CLI task needs contribution permission to the corresponding resource group. Follow this [article](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/connect-to-azure) to setup a service connection between Azure DevOps and your Azure Subscription.

For security reasons we don't deploy any settings or secrets from the bot project. Please ensure that required settings for your bit are configured in the [Azure Web App configuration](https://docs.microsoft.com/en-us/azure/app-service/configure-common), for example "MicrosoftAppPassword", "luis\_\_endpointKey", "cosmosDB\_\_authKey", ...
