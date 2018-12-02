# <%= botname %>
<%= description %>

This bot has been created using [Microsoft Bot Framework][1].

This samples shows how to:
- Use [LUIS][2] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user
- Demonstrate how to handle any unexpected errors


## Prerequisites
- [Node.js][4]
Ensure you have [Node.js][4] version 8.5 or higher installed.  To determine if Node.js is installed run the following from a shell window.
```bash
node --version
```
# To run the bot
Install modules
```bash
npm install
```
Start the bot.
```bash
npm start
```
Alternatively you can also run the watch script which will reload the bot when source code changes are detected.

Start the bot using a file watcher.
```bash
npm run watch
```

# Testing the bot using Bot Framework Emulator **V4**
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.1.0 or greater from [here][3]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `<%= botname %>` folder
- Select `<%= botname %>.bot` file

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

### Get a list of valid Azure subscriptions for your account
```bash
az login
az account list --output table
```
### Get a list of valid Azure locations
```bash
az account list-locations --output table
```

## Deploy using the msbot CLI
To deploy using `msbot clone` command an Azure `subscription-id` and Aure `location` is required.
```bash
msbot clone services -n <%= botname %> --subscriptionId <Azure-subscription-id> -l <Azure-location> --sdkLanguage "Node" -f deploymentScripts/msbotClone
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
- [TypeScript][2]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[2]: https://www.typescriptlang.org
[3]: https://www.typescriptlang.org/#download-links
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
