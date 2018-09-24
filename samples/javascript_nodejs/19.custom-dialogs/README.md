This sample demonstrates how to sub-class the Dialog class to create
different bot control mechanism like simple slot filling.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to samples/javascript_nodejs/19.custom-dialogs
    ```bash
    cd samples/javascript_nodejs/19.custom-dialogs
    ```
- Install modules and start the bot
    ```bash
    npm i && npm start
    ```

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is
a desktop application that allows bot developers to test and debug their bots on localhost
or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to javascript_nodejs/21.custom-dialogs
- Select custom-dialogs.bot file

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools - 

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```

# Custom Dialogs

Botbuilder provides a built-in base class called `Dialog`. By subclassing Dialog, developers
can create new ways to define and control dialog flows used by the bot. By adhering to the
features of this class, developers will create custom dialogs that can be used side-by-side
with other dialog types, as well as built-in or custom prompts.

This example demonstrates a custom Dialog class called `SlotFillingDialog`, which takes a
series of "slots" which define a value the bot needs to collect from the user, as well
as the prompt it should use. The bot will iterate through all of the slots until they are
all full, at which point the dialog completes.

# Further reading
- [Dialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/dialog)
- [WaterfallDialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall)
- [Manage complex conversation flows with dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-dialog-manage-complex-conversation-flow?view=azure-bot-service-4.0&tabs=javascript)