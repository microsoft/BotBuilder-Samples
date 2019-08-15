const { DocumentClient, UriFactory } = require("documentdb");
const uniqid = require("uniqid");
const {
  DB_SERVICE_ENDPOINT,
  AUTH_KEY,
  ACTIVITY_LOGS_DATABASE,
  ACTIVITY_LOGS_COLLECTION
} = process.env;

class UserConversationHistory {

  /**
   * Creates a new ConversationHistory instance.
   * @param settings Settings required for configuring an instance of ConversationHistory
   * including DocumentDb serviceEndpoint and authKey, as well as ACTIVITY_LOGS_DATABASE and 
   * ACTIVITY_LOGS_COLLECTION.
   */
  constructor(settings) {
    this.settings = { ...settings };
    this.client = new DocumentClient(settings.serviceEndpoint, { masterKey: settings.authKey });
  }

  async getIteratorResult(iterator) {
    return new Promise((resolve, reject) => {
      iterator.executeNext(function (err, result, responseHeaders) {
        if (err) {
          reject(err);
        }
        else {
          resolve({ result, responseHeaders });
        }
      });
    });
  }

 /**
 * Change the current transcript's UserId field. Was from.id and is now the auth provider's user id.
 * This ensures messages the user sent before signing in are included in future history retrievals.
 * @param context Current TurnContext
 * @param userState Current UserState object (we save the authUserId here, and use it for logging)
 * @param channelUserId Id of the user on the channel (required).  Before a user signs in, the log's
 * userId will be the From.Id of the incomging messages.  After the user has signed in, the log's
 * userId will be the user's id from the auth provider they had used to sign in.
 * @param authUserId Id of the user on the auth provider used to sign in (required).
 */
  async updateUserId(context, userState, channelUserId, authUserId) {

    if (!context) {
      throw new Error('Missing context.');
    }
    
    if (!userState) {
      throw new Error('Missing userState.');
    }

    if (!channelUserId) {
      throw new Error('Missing channelUserId.');
    }

    if (!authUserId) {
      throw new Error('Missing authUserId.');
    }

    const userIdProperty = userState.createProperty("AuthUserId");
    await userIdProperty.set(context, authUserId);

    const querySpec = {
      query: "SELECT * FROM root c WHERE c.userId = @userid",
      parameters: [
        {
          name: "@userid",
          value: channelUserId
        }
      ]
    };

    const collectionLink = UriFactory.createDocumentCollectionUri(this.settings.databaseId, this.settings.collectionId);
    const iterator = this.client.queryDocuments(collectionLink, querySpec);

    const q = await this.getIteratorResult(iterator);

    if (q.result) {
      for (var i = 0, len = q.result.length; i < len; i++) {
        var document = q.result[i];
        document.userId = authUserId;
        var isAccepted = this.client.replaceDocument(document._self, document, (d)=>{ });
      }
    }
  }

  /**
   * Retrieve's transcripts from CosmosDb by searching for records with a userId matching the userId
   * provided.
   * @param activity Activity being logged (required).
   * @param userId userId to retrieve transcripts for (required).
   * @param continuationToken To use while retrieving the next page of history messages.
   * @returns continuationToken (comsodb x-ms-continuation) and transcript (array of activities)
   */
  async getTranscript(activity, userId, continuationToken) {
    const MAX_ITEM_COUNT = 20;

    if (!activity) {
      throw new Error('Missing activity.');
    }

    if (!userId) {
      throw new Error('Missing userId.');
    }

    const options = {
      maxItemCount: MAX_ITEM_COUNT,
      continuation: continuationToken
    };

    // The transcript is retrieved based on the authenticated userid.
    // (see createTranscriptLogger where userId is based on authUserId present in channelData)
    const querySpec = {
      query: "SELECT * FROM root c WHERE c.userId = @userid ORDER BY c.activity.timestamp DESC",
      parameters: [
        {
          name: "@userid",
          value: userId
        }
      ]
    };

    const collectionLink = UriFactory.createDocumentCollectionUri(this.settings.databaseId, this.settings.collectionId);
    const iterator = this.client.queryDocuments(collectionLink, querySpec, options);

    const q = await this.getIteratorResult(iterator);
    const transcript = [];
    if (q.result) {
      q.result.forEach(function (record) {
        // Since we create a new conversation for the user every time they load WebChat,
        // this is necessary to ensure the activities will be accepted by directline for
        // the user's current conversation.
        record.activity.conversation.id = activity.conversation.id;
        //record.activity.channelData = null;
        if(!record.activity.id){
          record.activity.id = uniqid('g_');
        }

        transcript.push(record.activity);
      });
    }

    return { continuationToken: q.responseHeaders["x-ms-continuation"], transcript: transcript };
  }
}

module.exports = function createConversationHistory() {
  return new UserConversationHistory({
    serviceEndpoint: DB_SERVICE_ENDPOINT,
    authKey: AUTH_KEY,
    databaseId: ACTIVITY_LOGS_DATABASE,
    collectionId: ACTIVITY_LOGS_COLLECTION
  });
};