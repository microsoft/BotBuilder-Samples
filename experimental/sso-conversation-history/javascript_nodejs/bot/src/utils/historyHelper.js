const { ActivityTypes } = require('botbuilder');
const createConversationHistory = require('./createConversationHistory');
const botConst = require('../botConstants');

/**
 * If the user is signed in (determined based on checking authUserId of channelData), then
 * this method will send them history.  If this is the initialHistory, the HistoryContinuationToken
 * is cleared from userState, and the first page of history is sent to the user. 
 * @param context Current TurnContext
 * @param userState Current UserState object (we save the authUserId here, and use it for logging)
 * @param initialHistory When true, clear the HistoryContinuationToken from userState and send the 
 * first page of conversationHistory to the user.
 */
async function sendUserHistory(context, userState, initialHistory) {
    if (!context) {
        throw new Error('Missing context.');
    }

    if (!userState) {
        throw new Error('Missing userState.');
    }

    const continuationProperty = userState.createProperty("HistoryContinuationToken");

    if (initialHistory) {
        // Clear out the current continuation token, since this is an InitialHistory request
        // (subsequent requests for history pages will NOT contain this or it will be false, 
        // only the first request should send 'initialHistory:true')
        await continuationProperty.delete(context);
    }

    const activity = context.activity;
    // If we have an AuthUserId, it is used during logging (see HistoryMiddleware)
    let userId = null;
    if (activity.channelData && activity.channelData.authUserId) {
        userId = activity.channelData.authUserId;
    }
    else {
        // Do NOT send history if the user has not signed in
        return;
    }

    const continuation = await continuationProperty.get(context, "");
    const transcriptWithToken = await createConversationHistory().getTranscript(activity, userId, continuation);

    const connector = context.adapter.createConnectorClient(activity.serviceUrl);

    // Notify the WebChat control if there is no more history (so it can stop
    // sending requests for more history) by sending an Event.
    // See ConversaitonHistoryWeb.Views.Home.Bot.cshtml.
    if (!transcriptWithToken.continuationToken) {
        await context.sendActivity({
            type: ActivityTypes.Event,
            name: "historyComplete",
        });
    }

    if (transcriptWithToken.transcript) {
        let activities = transcriptWithToken.transcript.filter(item => item.type == ActivityTypes.Message);
        if (activities.length > 0) {

            await connector.conversations.sendConversationHistory(activity.conversation.id, { activities });
            // if there is no continuation token, then we would have already sent a historyComplete message
            // if history is not complete, notify WebChat that this particular page is complete
            if (transcriptWithToken.continuationToken) {
                await context.sendActivity({
                    type: ActivityTypes.Event,
                    name: "historyPageComplete",
                });
            }
        }
    }

    // Save the continuation token in userState, so it can be used in order to retrieve the next page 
    // of history.
    await continuationProperty.set(context, transcriptWithToken.continuationToken);
}

/**
 * Before a user signs in, all messages are logged using the From.Id or Recipient.Id.  After a user 
 * has signed in, we updated those logs and change the userId to the user's profile.id retreived 
 * from the auth provider. 
 * @param context Current TurnContext
 * @param profile Current user's provid
 * @param userState When true, clear the HistoryContinuationToken from userState and send the 
 * @param authProvider  of conversationHistory to the user.
 */
async function setUserIdAndSendHistory(context, profile, userState, authProvider) {
    // Store the id from the auth provider in channelData so it can be used
    // in history logging and retrieval
    context.activity.channelData = { ...context.activity.channelData, authUserId: profile.id };

    const history = createConversationHistory();

    // Update history record's userId so retrieving future history will also retrieve
    // messages communicated to the bot before being signed in
    await history.updateUserId(context, userState, context.activity.from.id, profile.id);

    // When the profile is fetched, send a welcome message and history based on
    // auth provider id.
    await context.sendActivity({
        text: `Welcome back, ${profile.name} id: ${profile.id} (via ${authProvider}). Note: I will keep a running total of numbers for you.)`,
        ...botConst.SUGGESTED_ACTIONS
    });

    await sendUserHistory(context, userState, true);
}

exports.sendUserHistory = sendUserHistory;
exports.setUserIdAndSendHistory = setUserIdAndSendHistory;
