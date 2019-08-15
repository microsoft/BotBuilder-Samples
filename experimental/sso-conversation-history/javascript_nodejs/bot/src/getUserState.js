const { UserState, MemoryStorage } = require('botbuilder');
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

const userState = new UserState(storage);

class AuthUserState extends UserState {
    getStorageKey(context) {
        const activity = context.activity;
        const channelId = activity.channelId;

        // If we have an AuthUserId, it is used to provide the AuthUserState key.
        // If there is no AuthUserId, then assume the user is not signed in and 
        // throw an exception.
        let userId = null;
        if (activity.channelData && activity.channelData.authUserId) {
            userId = activity.channelData.authUserId;
        }
        else {
            throw new Error('Must be authenticated to use AuthUserState.');
        }

        return `${ channelId }/users/${ userId }/${ this.namespace }`
    }
}
const authUserState = new AuthUserState(storage);

module.exports = function getUserState() {
    return { userState, authUserState };
}