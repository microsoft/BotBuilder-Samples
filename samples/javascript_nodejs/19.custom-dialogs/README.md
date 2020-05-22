# custom dialogs

Bot Framework v4 custom dialogs bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to sub-class the `Dialog` class to create different bot control mechanism like simple slot filling.

BotFramework provides a built-in base class called `Dialog`. By subclassing `Dialog`, developers can create new ways to define and control dialog flows used by the bot.

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

- In a terminal, navigate to `samples/javascript_nodejs/19.custom-dialogs`

    ```bash
    cd samples/javascript_nodejs/19.custom-dialogs
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

- Install the Bot Framework Emulator version 4.9.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Custom Dialogs

BotFramework provides a built-in base class called `Dialog`. By subclassing Dialog, developers
can create new ways to define and control dialog flows used by the bot. By adhering to the
features of this class, developers will create custom dialogs that can be used side-by-side
with other dialog types, as well as built-in or custom prompts.

This example demonstrates a custom Dialog class called `SlotFillingDialog`, which takes a
series of "slots" which define a value the bot needs to collect from the user, as well
as the prompt it should use. The bot will iterate through all of the slots until they are
all full, at which point the dialog completes.

## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Dialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/dialog)
- [Manage complex conversation flows with dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-dialog-manage-complex-conversation-flow?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
