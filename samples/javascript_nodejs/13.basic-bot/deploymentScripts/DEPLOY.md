# How To Deploy
This bot relies on the [LUIS.ai][1] cognitive service to function. This document will show how to use simple command line tools to setup LUIS for use with this bot.

# Install required tools
To successfully setup and configure the services this bot depends on, you need to install the MSBOT, LUIS, and Ludown CLI tools.  See the documentation for the [Bot Framework CLI Tools][5] for additional information on what CLI tools are available to help you build your bot.

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot luis-apis ludown
```

# Keeping track of service references using .bot file
We highly recommend you can keep track of all the services your bot depends on in a .bot file. You can use either the `msbot` CLI tool or use [Bot Framework Emulator][7] to manage your .bot file.  This project was created with a bot file named [`basic-bot.bot`](../basic-bot.bot)

# Configure all required servies
You can use [MSBOT][5] to create and configure the required services for this bot. To do this, simply run the following command from a terminal.
You can get your LUIS authoring key by following steps [here][8] and get your Azure subscription ID by following steps [here][9].

```bash
> cd samples/javascript_nodejs/13.basic-bot

> msbot clone -n <YOUR BOT NAME> -f deploymentScripts/msbotClone --luisAuthoringKey <LUIS-KEY> --subscriptionId <AZURE-SUBSCRIPTION-ID>
```

# Using the CLI tools to create and configure services
The LUIS application created for this bot is described in a markdown based .lu file(s) [here](../dialogs/greeting/resources/main.lu). To update your published LUIS application with any changes made to the .lu files,  run the following commands. 

```bash
> cd samples/javascript_nodejs/13.basic-bot
```

1. To parse the .lu files to a LUIS model: 
    ```bash
    > ludown parse toluis --in dialogs/greeting/resources/main.lu -o cognitiveModels/ --out basicBot.luis -n 'basic-bot-LUIS' -d 'Basic bot Bot Builder V4 sample.' --verbose
    ```
2. To create a new LUIS application using the LUIS model generated in step 1 and update the .bot file with the LUIS service configuration: 
    ```bash
    > luis import application --in cognitiveModels/basicBot.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --msbot --endpointRegion --region <LUIS-AUTHORING-REGION> | msbot connect luis --stdin
    ```
    You can obtain your LUIS authoring key by following instructions [here][8]
    LUIS authoring regions are listed [here][10]. They can be one of westus | westeurope | australiaeast.
3. To train the LUIS application, 
    ```bash
    > msbot get basic-bot-LUIS | luis train version --wait --stdin
    ```
4. To publish the LUIS application, 
    ```bash
    > msbot get basic-bot-LUIS | luis publish version --stdin
    ```

See [Bot Builder tools](https://github.com/microsoft/botbuilder-tools) to learn more about the Bot Builder CLI tools.

[1]: https://www.luis.ai
[2]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-concept-intelligence
[3]: https://portal.azure.com
[4]: https://azure.microsoft.com/en-us/get-started/
[5]: https://github.com/microsoft/botbuilder-tools
[6]: https://dev.botframework.com
[7]: https://www.github.com/microsoft/botframework-emulator
[8]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings
[9]: https://blogs.msdn.microsoft.com/mschray/2016/03/18/getting-your-azure-subscription-guid-new-portal/