This sample shows how to use the prompt classes included in `botbuilder-dialogs`.
This bot will ask for the user's name and age, then store the responses. It demonstrates
a multi-turn dialog flow using a text prompt, a number prompt, and state accessors to
store and retrieve values.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs/5.multiturn-prompts-bot
    ```bash
    cd javascript_nodejs/5.multiturn-prompts-bot
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
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot
developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to javascript_nodejs/5.multiturn-prompts-bot folder
- Select multiturn-prompts-bot.bot file

# Prompts
A conversation between a bot and a user often involves asking (prompting) the user for information, parsing the user's response,
and then acting on that information. This sample demonstrates how to prompt users for information using the different prompt types
included in the [botbuilder-dialogs](https://github.com/microsoft/botbuilder-js/tree/master/libraries/botbuilder-dialogs) library
and supported by the SDK.

The `botbuilder-dialogs` library includes a variety of pre-built prompt classes, including text, number, and datetime types. This
sample demonstrates using a text prompt to collect the user's name, then using a number prompt to collect an age.

# Further reading
- [Prompt types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Waterfall dialogs](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall)
- [Ask the user questions](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-waterfall?view=azure-bot-service-4.0&tabs=jstab)
