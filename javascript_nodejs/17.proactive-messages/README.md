This sample demonstrates how to send proactive messages to users by
capturing a conversation reference, then using it later to initialize
outbound messages.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs/17.proactive-messages
    ```bash
    cd javascript_nodejs/17.proactive-messages
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
[Microsoft Bot Framework emulator](https://github.com/microsoft/botframework-emulator) is
a desktop applicationthat allows bot developers to test and debug their bots on localhost
or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework emulator
- File -> Open Bot Configuration and navigate to javascript_nodejs/17.proactive-messages
- Select proactive-messages.bot file

# Proactive Messages


# Further reading
- [Send proactive messages](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0&tabs=js)

TODO: Add more links here