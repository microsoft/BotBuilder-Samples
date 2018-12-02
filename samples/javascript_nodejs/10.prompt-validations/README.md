# prompt validation
Bot Framework v4 prompt validation bot sample

A conversation between a bot and a user often involves asking (prompting) the user for information.  This sample shows how to use the prompt classes included in `botbuilder-dialogs`.  This bot will ask for multiple pieces of information from the user, each using a different type of prompt, each with its own validation rules. This sample also demonstrates using the `ComponentDialog` class to encapsulate related sub-dialogs.

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
- In a terminal, navigate to `samples/javascript_nodejs/10.prompt-validations`
    ```bash
    cd samples/javascript_nodejs/10.prompt-validations
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
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `javascript_nodejs/10.prompt-validations`
- Select `prompt-validations.bot` file

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Prompts
A conversation between a bot and a user often involves asking (prompting) the user for information,
parsing the user's response, and then acting on that information. This sample demonstrates how to
prompt users for information and validate the incoming responses using the different prompt types included in the
[botbuilder-dialogs](https://github.com/Microsoft/botbuilder-js/tree/master/libraries/botbuilder-dialogs)
library.

The `botbuilder-dialogs` library includes a variety of pre-built prompt classes, including text, number,
and datetime types. In this sample, each prompt is wrapped in a custom class that includes a validation
function. These prompts are chained together into a `WaterfallDialog`, and the final results are stored
using the state manager.

# Further reading
- [Prompt types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
