# How To Deploy
This bot relies on the [LUIS][1] cognitive service to function. This document will show how to use simple command line tools to setup LUIS for use with this bot.

## Install required tools
To successfully setup and configure the services this bot depends on, you need to install the MSBOT, LUIS, and Ludown CLI tools.  See the documentation for the [Bot Framework CLI Tools][5] for additional information on what CLI tools are available to help you build your bot.

```bash
npm install -g chatdown msbot ludown luis-apis qnamaker botdispatch luisgen
```
**NOTE**: You must have [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest#install-or-update) installed at this point. Also, the bot service extension, at least 0.4.1 version must be installed. Follow these steps to install it.

```bash
az extension remove -n botservice
az extension add-n botservice
```

## Keeping track of service references using .bot file
We highly recommend you can keep track of all the services your bot depends on in a `.bot` file. You can use either the `msbot` CLI tool or use [Bot Framework Emulator][7] to manage your `.bot` file.

## Configure all required services
You can use [MSBOT][5] to create and configure the required services for this bot. To do this, simply run the following command from a terminal.
You can get your LUIS authoring key by following steps [here][8] and get your Azure subscription ID by following steps [here][9].

```bash
> cd ./samples/javascript_typescript/52.enterprise-bot

> msbot clone services --name <YOUR BOT NAME> --luisAuthoringKey <LUIS-KEY> --folder deploymentScripts/msbotClone --location westus --verbose
```
**WARNING** please use a short name (8 char max). With longer name the script fails to create the blob-storage.

**NOTE**: By default your Luis Applications will be deployed to your free starter endpoint. An Azure LUIS service will be deployed along with your bot but you must manually add and publish to it from the luis.ai portal and update your key in the .bot file.

# Updates to models
The LUIS application created for this bot is described in a markdown based .lu file [here](../cognitiveModels/LUIS/General.lu). To update your published LUIS application with any changes made to the .lu files,  run the following commands. 

```bash
> cd javascript_typescript/52.enterprise-bot
> ludown parse toluis --in cognitiveModels/LUIS/General.lu -o cognitiveModels/LUIS --out General.luis
> msbot get service --name "basic-bot-LUIS" | luis rename version --newVersionId 0.1_old --stdin
> msbot get service --name "basic-bot-LUIS" | luis import version --stdin --in cognitiveModels/LUIS/General.luis
> msbot get service --name "basic-bot-LUIS" | luis delete version --stdin --versionId 0.1_old
> msbot get service --name "basic-bot-LUIS" | luis train version --wait --stdin
> msbot get service --name "basic-bot-LUIS" | luis publish version --stdin
```

[1]: https://www.luis.ai
[2]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-concept-intelligence
[3]: https://portal.azure.com
[4]: https://azure.microsoft.com/en-us/get-started/
[5]: https://github.com/microsoft/botbuilder-tools
[6]: https://dev.botframework.com
[7]: https://www.github.com/microsoft/botframework-emulator
[8]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings
[9]: https://blogs.msdn.microsoft.com/mschray/2016/03/18/getting-your-azure-subscription-guid-new-portal/