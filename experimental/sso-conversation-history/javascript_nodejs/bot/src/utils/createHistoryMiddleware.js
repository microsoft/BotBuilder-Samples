class HistoryMiddleware {

  /**
   * This middleware is responsible for retrieving AuthUserId from UserState, and adding it to 
   * activity channelData (the value is used for logging, and history retrieval).
   * @param userState UserState is utilized to retrieve AuthUserId from UserState (if present).
   */
  constructor(userState) {
    if (!userState) {
      throw new Error(`userState is required`);
    };
    this.userState = userState
  }

  async onTurn(context, next) {
    if (!context) {
      throw new Error(`Context is required`);
    };

    // Do NOT allow incoming activities to contain an authUserId in channelData. 
    // This implementation is using this value internally for logging purposes.
    if(context.activity.channelData && context.activity.channelData.authUserId){
      throw new Error("Activity cannot contain channelData.authUserId.");
    }

    // Retrieve the AuthUserId property for this user, and ensure it is present in 
    // Activity's ChannelData so it can be used during logging.
    const userIdProperty = this.userState.createProperty("AuthUserId");
    let authUserId = await userIdProperty.get(context, "");
    if (authUserId) {
      context.activity.channelData = {...context.activity.channelData, authUserId };
    }

    await context.onSendActivities(async (context, activities, nextSend) => {
      // Add authUserId to channelData so outgoing messages are logged correctly.
      let authUserId = await userIdProperty.get(context, "");
      if (authUserId) {
        for (const activity of activities) {
          activity.channelData = {...activity.channelData, authUserId };
        }
      }
      return await nextSend();
    }, userIdProperty);

    // By calling next() you ensure that the next Middleware is run.
    await next();
  };
}

module.exports = function createHistoryMiddleware(userState) {
  return new HistoryMiddleware(userState);
};