const { UserState } = require('botbuilder');
const { CosmosDbStorage } = require("botbuilder-azure");

const {
    DB_SERVICE_ENDPOINT,
    AUTH_KEY,
    BOT_STATE_DATABASE,
    BOT_STATE_COLLECTION,
  } = process.env;

  
class AuthUserState extends UserState {

   /**
   * When a user is signed in, the channelData will have their auth user id (see HistoryMiddleware).
   * The Auth User Id is used as the key for AuthUserState.  
   * An exception is thrown if channelData does NOT have authUserId, and an AuthUserState property
   * is used.
   * @param context Current ITurnContext.
   */
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

module.exports = function createUserStates() {
    const storage = new CosmosDbStorage({
        serviceEndpoint: DB_SERVICE_ENDPOINT,
        authKey: AUTH_KEY,
        databaseId: BOT_STATE_DATABASE,
        collectionId: BOT_STATE_COLLECTION
    })

    // UserState is used to store the AuthUserId for this session, and is keyed on
    // the user's generated id (see rest-api/generateDirectLineToken.js)
    const userState = new UserState(storage);
    
    // AuthUserState is used for storing information specific to users who have 
    // signed in.  It is keyed on activity.channelData.authUserId, which is retrieved
    // from UserState and added to incoming messages within createHistoryMiddleware.js
    const authUserState = new AuthUserState(storage);
    return { userState, authUserState };
}