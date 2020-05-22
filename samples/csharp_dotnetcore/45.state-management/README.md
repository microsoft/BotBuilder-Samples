# Save user and conversation data

This sample demonstrates how to save user and conversation data in an ASP.Net Core 2 bot.
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

## Further reading

- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)
