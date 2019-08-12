// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const path = require('path');
const restify = require('restify');
const { CosmosDbStorage } = require("botbuilder-azure");
const V3StorageProvider = require('botbuilder-azure-v3');
const { StorageMapper } = require('./V4V3StorageMapper');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter, ConversationState } = require('botbuilder');

// Modified in order to match V3-style storage key
const { V3UserState } = require('./V4V3UserState/lib/V3UserState');

// This bot's main dialog.
const { StateManagementBot } = require('./bots/stateManagementBot');

// Read environment variables from .env file
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Create HTTP server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
});

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

/*-----------------------------------------------------------------------------
  CosmosDB Configuration
-----------------------------------------------------------------------------*/

// V4 client for CosmosDB storage provider
const cosmosStorage = new CosmosDbStorage({
    serviceEndpoint: process.env.COSMOS_DB_SERVICE_ENDPOINT,  // obtain from Azure Portal
    authKey: process.env.COSMOS_AUTH_KEY,  // obtain from Azure Portal
    databaseId: process.env.COSMOS_DATABASE, // user defined
    collectionId: process.env.COSMOS_COLLECTION // user defined
});

// V3 client for CosmosDB storage provider
const docDbClient = new V3StorageProvider.DocumentDbClient({
    host: process.env.COSMOS_DB_SERVICE_ENDPOINT,
    masterKey: process.env.COSMOS_AUTH_KEY,
    database: process.env.COSMOS_DATABASE,
    collection: process.env.COSMOS_COLLECTION
});

// V3 Azure Cosmos client
const cosmosStorageClient = new V3StorageProvider.AzureBotStorage({ gzipData: false }, docDbClient);


/*-----------------------------------------------------------------------------
  Table Storage Configuration
-----------------------------------------------------------------------------*/

// V3 Azure Table Storage keys
// Table storage
const tableName = process.env.TABLE_STORAGE_TABLE_NAME; // user defined
const storageName = process.env.TABLE_STORAGE_STORAGE_NAME; // obtain from Azure Portal
const storageKey = process.env.TABLE_STORAGE_STORAGE_KEY; // obtain from Azure Portal

// V3 Azure table storage client
const azureTableClient = new V3StorageProvider.AzureTableClient(tableName, storageName, storageKey);
const tableStorage = new V3StorageProvider.AzureBotStorage({gzipData: false}, azureTableClient);


/*-----------------------------------------------------------------------------
  SQL Database Configuration
-----------------------------------------------------------------------------*/

const sqlConfig = {
    userName: process.env.SQL_USER_NAME, // obtain from Azure Portal
    password: process.env.SQL_PASSWORD, // obtain from Azure Portal
    server: process.env.SQL_SERVER_HOST, // obtain from Azure Portal
    enforceTable: true, // If this property is not set to true it defaults to false. When false if the specified table is not found, the bot will throw an error.
    options: {
        database: process.env.SQL_DATABASE_NAME, // user defined
        table: process.env.SQL_TABLE_NAME, // user defined
        encrypt: true,
        rowCollectionOnRequestCompletion: true
    }
}

const sqlClient = new V3StorageProvider.AzureSqlClient(sqlConfig);
sqlClient.initialize((err) => {  // create the SQL table if it doesn't already exist
    if (err) console.log(err);
});
const sqlStorage = new V3StorageProvider.AzureBotStorage({ gzipData: false }, sqlClient);


/*-----------------------------------------------------------------------------
  Connect storage to bot
-----------------------------------------------------------------------------*/

/*** Pass current storage provider to StorageMapper cosmosStorageClient ***/
/*** possible values: cosmosStorageClient, tableStorage, sqlStorage     ***/
const storageMapper = new StorageMapper(cosmosStorageClient);
const userState = new V3UserState(storageMapper);

// Create conversation and user state with in-memory storage provider. This uses V4 state.
const conversationState = new ConversationState(cosmosStorage);

// Create the bot.
const bot = new StateManagementBot(conversationState, userState);

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log
    // NOTE: In production environment, you should consider logging this to Azure
    //       application insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    const onTurnErrorMessage = `Sorry, it looks like something went wrong!`;
    await context.sendActivity(onTurnErrorMessage, onTurnErrorMessage);
    // Clear out state
    await conversationState.delete(context);
};

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // Route to main dialog.
        await bot.run(context);
    });
});
