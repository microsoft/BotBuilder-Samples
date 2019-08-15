const { CosmosClient } = require("@azure/cosmos");
const { TranscriptLoggerMiddleware } = require('botbuilder-core');
const { ActivityTypes } = require('botbuilder');
const {
  DB_SERVICE_ENDPOINT,
  AUTH_KEY,
  ACTIVITY_LOGS_DATABASE,
  ACTIVITY_LOGS_COLLECTION
} = process.env;

class CosmosDbTranscriptLogger {
  /**
   * Creates a new CosmosDbTranscriptLogger instance.  This logger ensures there is a userId
   * field with a value of the AuthUserId from the sign in provider, if present. If not present,
   * userId will be the activity.from.id or the activity.recipient.id.
   * @param settings Settings required for configuring an instance of CosmosDbTranscriptLogger
   * which uses a CosmosDlient. CosmosDb { serviceEndpoint, authKey, databaseId, collectionId }
   */
  constructor(settings) {
      this.settings = { ...settings };
      this.client = new CosmosClient({ endpoint: settings.serviceEndpoint, key: settings.authKey });
  }

  /**
   * Log an activity to cosmosdb.
   * @param activity Activity to be logged.
   */
  async logActivity(activity) {
      if (!activity) {
          throw new Error('Missing activity.');
      }

      // Only log messages
      if(activity.type === ActivityTypes.Message) {
        // If we have an AuthUserId, use it during logging (so future signed in users will have history)
        let userId = null;
        if(activity.channelData && activity.channelData.authUserId){
          userId = activity.channelData.authUserId;
        }

        if(!userId){
          // There is no AuthUserid, so use the recipient.id or the from.id (depending on which type of message)
          userId = activity.recipient.role === 'user' ? activity.recipient.id : activity.from.id;
        }

        const { database } = await this.client.databases.createIfNotExists({ id: this.settings.databaseId });
        const { container } = await database.containers.createIfNotExists({ id: this.settings.collectionId });

        // userId is the field searched while retrieving transcripts for ConversationHistory
        container.items.create({ id: activity.id, userId:userId, activity: activity });
      }
  }
}

module.exports = function createTranscriptLogger() {
  const cosmosDbLogger = new CosmosDbTranscriptLogger({
    serviceEndpoint: DB_SERVICE_ENDPOINT,
    authKey: AUTH_KEY,
    databaseId: ACTIVITY_LOGS_DATABASE,
    collectionId: ACTIVITY_LOGS_COLLECTION
  });
  return new TranscriptLoggerMiddleware(cosmosDbLogger);
};