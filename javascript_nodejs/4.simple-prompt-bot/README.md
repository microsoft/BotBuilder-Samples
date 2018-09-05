This sample shows how to use the prompts classes included in `botbuilder-dialogs`. This bot will ask for the user's name, then store the response. It demonstrates a 2-step dialog flow using a prompt, as well as using the state accessors to store and retrieve values.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs/4.simple-prompt-bot
    ```bash
    cd javascript_nodejs/4.simple-prompt-bot
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
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples\2.echobot-with-state folder
- Select simple-prompt-bot.bot file

# Prompts

A conversation between a bot and a user often involves asking (prompting) the user for information, parsing the user's response, and then acting on that information. This sample demonstrates how to prompt users for information using the different prompt types included in the [botbuilder-dialogs](https://github.com/Microsoft/botbuilder-js/tree/master/libraries/botbuilder-dialogs) library and supported by the SDK.

The `botbuilder-dialogs` library includes a variety of pre-built prompt classes, including text, number, and datetime types. This sample demonstrates using a single text prompt to collect the user's name. For an example that uses multiple prompts of different types, see [sample 5](../5.multi-prompt-bot/).

# Further reading
- [Prompt types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)