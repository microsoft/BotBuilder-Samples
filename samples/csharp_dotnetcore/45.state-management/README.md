# Save user and conversation data

This sample demonstrates how to save user and conversation data in a bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.

By default this bot uses MemoryStorage for Conversation and User state.
Memory Storage is great for testing purposes, but in a production scenario you will need to use persistent storage.

Startup.cs contains commented examples of using Azure Blob Storage or Cosmos DB instead.

# Azure Blob Storage

To use Azure Blob Storage, create a Blob Storage account in your Azure subscription. You can then use the following code to
create your storage layer, passing in your Blob Storage connection string and a blob container name.

***Note: You do not need to create the container manually, the bot will create the container for you if it does not exist.***

```cs
    var storage = new AzureBlobStorage("<blob-storage-connection-string>", "bot-state");
```

# CosmosDB Storage

To use CosmosDB Storage, you need to create a CosmosDB instance in your Azure subscription. You can then use the following code to
create your storage layer.

***Note: It is your responsibility to create an appropriate database within your CosmosDB instance. However, you should **not**
create the container within the database, as the bot will do this for you and ensure the container is configured correctly.***

```cs
    var cosmosDbStorageOptions = new CosmosDbPartitionedStorageOptions()
    {
        CosmosDbEndpoint = "<endpoint-for-your-cosmosdb-instance>",
        AuthKey = "<your-cosmosdb-auth-key>",
        DatabaseId = "<your-database-id>",
        ContainerId = "<cosmosdb-container-id>"
    };

    var storage = new CosmosDbPartitionedStorage(cosmosDbStorageOptions);
```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/45.state-management`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/45.state-management` folder
  - Select `StateMangementBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Bot State

A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. Depending on what your bot is used for, you may even need to keep track of state or store information for longer than the lifetime of the conversation. A bots state is information it remembers in order to respond appropriately to incoming messages. The Bot Builder SDK provides classes for storing and retrieving state data as an object associated with a user or a conversation.

## Further reading

- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-state?view=azure-bot-service-4.0)
