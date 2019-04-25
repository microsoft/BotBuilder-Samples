# Save user and conversation data

This sample demonstrates how to save user and conversation data in a Node.js bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.

## Prerequisites

- Node.js version 10.14.1 or higher.

    ```bash
    # determine node version
    node --version
    ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/45.state-management`

    ```bash
    cd samples/javascript_nodejs/45.state-management
    ```

- Install modules

    ```bash
    npm install
    ```

- Run the sample

    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

## Further reading

- [Azure Bot Service Introduction][21]
- [Bot State][7]
- [Write directly to storage][8]
- [Managing conversation and user state][9]
- [Restify][30]
- [dotenv][31]

[3]: https://aka.ms/botframework-emulator
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/azure/bot-service/bot-builder-storage-concept
[8]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-storage?tabs=js
[9]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-state?tabs=js
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[40]: https://aka.ms/azuredeployment
