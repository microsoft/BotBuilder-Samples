const { ConversationState, MemoryStorage } = require('botbuilder');
const { CosmosDbStorage } = require("botbuilder-azure");
const path = require('path');

const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

var storage = new CosmosDbStorage({
    serviceEndpoint: process.env.DB_SERVICE_ENDPOINT, 
    authKey: process.env.AUTH_KEY, 
    databaseId: process.env.BOT_STATE_DATABASE,
    collectionId: process.env.BOT_STATE_COLLECTION
})

const conversationState = new ConversationState(storage);

module.exports = () => conversationState;
