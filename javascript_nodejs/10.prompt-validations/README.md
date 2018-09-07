This sample shows how to use the prompts classes included in `botbuilder-dialogs`.
This bot will ask for multiple pieces of information from the user, each using a
different type of prompt, each with its own validation rules. This sample also
demonstrates using the `ComponentDialog` class to encapsulate related sub-dialogs.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs/10.prompt-validations
    ```bash
    cd javascript_nodejs/10.prompt-validations
    ```
    - Point to the MyGet feed 
    ```bash
    npm config set registry https://botbuilder.myget.org/F/botbuilder-v4-js-daily/npm/
    ```
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is
a desktop application that allows bot developers to test and debug their bots on localhost
or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework emulator
- File -> Open Bot Configuration and navigate to javascript_nodejs/10.prompt-validations
- Select prompt-validations-bot.bot file

# Prompts

A conversation between a bot and a user often involves asking (prompting) the user for information,
parsing the user's response, and then acting on that information. This sample demonstrates how to
prompt users for information using the different prompt types included in the
[botbuilder-dialogs](https://github.com/Microsoft/botbuilder-js/tree/master/libraries/botbuilder-dialogs)
library.

The `botbuilder-dialogs` library includes a variety of pre-built prompt classes, including text, number,
and datetime types. In this sample, each prompt is wrapped in a custom class that includes a validation
function. These prompts are chained together into a `WaterfallDialog`, and the final results are stored
using the state manager.

# Further reading
- [Prompt types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)

TODO: Add more links here