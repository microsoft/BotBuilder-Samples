# How To Deploy
This bot relies on the [LUIS][1] cognitive service to function. This document will show how to use simple command line tools to setup LUIS for use with this bot.

# Install required tools
To successfully setup and configure the services this bot depends on, you need to install the MSBOT, LUIS, and Ludown CLI tools.  See the documentation for the [Bot Framework CLI Tools][5] for additional information on what CLI tools are available to help you build your bot.

```bash
npm i -g msbot luis-apis ludown
```

# Keeping track of service references using .bot file
We highly recommend you can keep track of all the services your bot depends on in a .bot file. You can use either the `msbot` CLI tool or use [Bot Framework Emulator][7] to manage your .bot file.  This project was created with a bot file named `basic-bot.bot`

# Configure all required servies
You can use [MSBOT][5] to create and configure the required services for this bot. To do this, simply run the following command from a terminal.
You can get your LUIS authoring key by following steps [here][8] and get your Azure subscription ID by following steps [here][9].

```bash
> cd javascript_nodejs/13.basic-bot

> msbot clone -n <YOUR BOT NAME> -f deploymentScripts/msbotClone --luisAuthoringKey <LUIS-KEY> --subscriptionId <AZURE-SUBSCRIPTION-ID>
```

# Updates to models
The LUIS application created for this bot is described in a markdown based .lu file [here](../dialogs/greeting/resources/greeting.lu). To update your published LUIS application with any changes made to the .lu files,  run the following commands. 

```bash
> cd javascript_nodejs/13.basic-bot
> ludown parse toluis --in dialogs/resources/greeting.lu -o cognitiveModels --out greeting.luis
> msbot get service --name "basic-bot-LUIS" | luis rename version --newVersionId 0.1_old --stdin
> msbot get service --name "basic-bot-LUIS" | luis import version --stdin --in cognitiveModels\greeting.luis
> msbot get service --name "basic-bot-LUIS" | luis delete version --stdin --versionId 0.1_old
> msbot get service --name "basic-bot-LUIS" | luis train version --wait --stdin
> msbot get service --name "basic-bot-LUIS" | luis publish version --stdin

[1]: https://www.luis.ai
[2]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-concept-intelligence
[3]: https://portal.azure.com
[4]: https://azure.microsoft.com/en-us/get-started/
[5]: https://github.com/microsoft/botbuilder-tools
[6]: https://dev.botframework.com
[7]: https://www.github.com/microsoft/botframework-emulator
[8]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings
[9]: https://blogs.msdn.microsoft.com/mschray/2016/03/18/getting-your-azure-subscription-guid-new-portal/