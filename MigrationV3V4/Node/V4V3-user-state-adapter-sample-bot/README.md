# Save user and conversation data

This sample demonstrates how to use v3 user state from a v4 bot (read, write & delete)
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
    cd MigrationV3V4/Node/V4V3-user-state-adapter-sample-bot/
    ```

- Install modules (run 'npm install') in the following locations:

    ```bash
    root
    /V4V3StorageMapper
    /V4V3UserState
    ```

- Build the StorageMapper and UserState modules; run 'npm run build' or 'tsc' in the following locations:

    ```bash
    /V4V3StorageMapper
    /V4V3UserState
    ```

- DB Config setup

    Copy the contents of the '.env.example' file. Create a new file called '.env' and past the contents into this file. Fill in the values for the storage provider(s) that you are using. Username, password and host information can be found in the Azure portal under the section for your particular storage provider (Cosmos DB, Table storage or SQL database). Table and collection names are user-defined.
    
- Set the bot's storage provider

    Open the 'index.js' file in the project root. Towards the beginning of the file (lines ~38-98) you will see configurations for each storage provider, as noted in the   comments. They read in the config values from the .env file via Node process.env.

    Specify which storage provider you want your bot to use by passing in the storage client instance of your choice to the 'StorageMapper' adapter (~line 107). It is set to use Cosmos DB by default. The possible values are:

    ```bash
    cosmosStorageClient
    tableStorage
    sqlStorage
    ```

- Start the application. For the project root, run:

    ```bash
    npm run start
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
