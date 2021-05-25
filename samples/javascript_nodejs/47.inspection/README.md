# Inspection Bot

Bot Framework v4 Inspection Middleware sample.

This bot demonstrates a feature called Inspection. This feature allows the Bot Framework Emulator to debug traffic into and out of the bot in addition to looking at the current state of the bot. This is done by having this data sent to the emulator using trace messages.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back. Included in this sample are two counters maintained in User and Conversation state to demonstrate the ability to look at state.

This runtime behavior is achieved by simply adding a middleware to the bot. In this sample you can find that being done in the `AdapterWithInspection` class.

More details are available [here](https://github.com/microsoft/BotFramework-Emulator/blob/master/content/CHANNELS.md)

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/47.inspection`

    ```bash
    cd samples/javascript_nodejs/47.inspection
    ```

- Install modules

    ```bash
    npm install
    ```

- Start the bot

    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`


### Special Instructions for Running Inspection

(Note the follow applies if you are using two emulators. More recnt builds hide the need to type /INSPECT open.)

- Lanuch a second Emulator - this will act as the debugger
- In this second Emulator type /INSPECT open
- The result is a trace activity which contains a statement that looks like /INSPECT attach < identifier >
- In the chat session that you want to inspect type this command exact as it is (you can use cut and paste)
- Now all the traffic will be replicated (as trace activities) to the Emulator that sent the /INSPECT open command

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
