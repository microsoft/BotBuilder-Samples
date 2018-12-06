# echobot
Bot Framework v4 echo bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to create a simple bot that accepts input from the user and echoes it back.

## Prerequisites
- [Node.js][4] version 8.5 or higher

```bash
# determine node version
node --version
```

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/02.a.echobot`
    ```bash
    cd samples/javascript_nodejs/02.a.echobot
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.1.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `echobot` folder
- Select `echobot.bot` file

# Deploy the bot to Azure
## Prerequisites
In order to deploy your bot to Microsoft Azure, you must have:
- Azure CLI installed
- msbot CLI installed
- An Azure `subscription-id`
- An Azure `location`

### Installing the Azure CLI
- Navigate to the [Azure CLI portal][8].
- Click the installation instructions for your development environment.

### Installing the msbot CLI
You will use `msbot` CLI to deploy your bot to Microsoft Azure.
```bash
npm i -g msbot
```
`msbot` requires an Azure `subscription-id` and an Azure `location` to deploy.  Use `Azure CLI` to login to Azure, get a list of your subscriptions and get a list of supported locations.

### To get a list of valid Azure subscriptions for your account
```bash
az login
az account list --output table
```
### To get a list of valid Azure locations
```bash
az account list-locations --output table
```


## Deploy using the msbot CLI
To deploy using `msbot clone` command an Azure subscription-id and Aure location is required.
```bash
msbot clone services -n echobot --subscriptionId <Azure-subscription-id> -l <Azure-location> --sdkLanguage "Node" -f deploymentScripts/msbotClone
```
When `msbot` runs, it will display a list of resources and services it will provision as part of the bot deployment.  It will prompt for confirmation in order to proceed with the deployment.  Example output from deploying a bot named `ms-chat-bot` in `westus` looks as follows:
```
Service                             Location   SKU              Resource Group
 Azure App Site Plan                westus     S1               my-chat-bot
 Azure AppInsights Service          West US 2  F0               my-chat-bot
 Azure Blob Storage Service         westus     Standard_LRS     my-chat-bot
 Azure Bot Service Registration     Global                      my-chat-bot
 Azure WebApp Service (Bot)         westus                      my-chat-bot
Would you like to perform this operation? [y/n]
```

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
