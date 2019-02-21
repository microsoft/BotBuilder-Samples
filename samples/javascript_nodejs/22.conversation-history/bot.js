// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Import required Bot Framework classes.
const { ActivityTypes } = require('botbuilder');

class ConversationHistoryBot {
    /**
     *
     * @param {AzureBlobTranscriptStore} transcriptStore is where 
     *                    the bot conversation transcript is stored 
     *                    and retrieved with a call to getTranscriptActivities.
     */
    constructor(transcriptStore) {
        // The transcript store will be queried to retrieve the transcript
        // if the bot receives '!history' from a user.
        this.transcriptStore = transcriptStore;
    }
    /**
     *
     * @param {TurnContext} context on turn context object.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            if (turnContext.activity.text == "!history")
            {
                // Retrieve the activities from the Transcript (blob store) and send them over to the channel when 
                // a request to upload history arrives. 
                //This could be an event or a special message activity as above.

                // Create the connector client to send transcript activities
                let connector = turnContext.adapter.createConnectorClient(turnContext.activity.serviceUrl);

                // Get all the message type activities from the Transcript.
                let continuationToken = "";
                var count = 0;

                // WebChat and Emulator require modifying the activity.Id to display the same activity again within the same chat window
                let updateActivities = [ 'webchat', 'emulator', 'directline' ].includes(turnContext.activity.channelId);
                let incrementId = 0;
                if (updateActivities && turnContext.activity.id.includes("|"))
                {
                    incrementId = parseInt(turnContext.activity.id.split('|')[1]) || 0;
                }

                do
                {
                    // Transcript activities are retrieved in pages.  When the continuationToken is null, 
                    // there are no more activities to be retireved for this conversation.
                    var pagedTranscript = await this.transcriptStore.getTranscriptActivities(turnContext.activity.channelId, turnContext.activity.conversation.id, continuationToken);
                    let activities = pagedTranscript.items.filter(item => item.type == ActivityTypes.Message);

                    if (updateActivities)
                    {
                        activities.forEach(function(a) {
                            incrementId++;
                            a.id = `${turnContext.activity.conversation.id}|${incrementId.toString().padStart(7, '0')}`;
                            a.timestamp = new Date().toISOString();
                            a.channelData = []; // WebChat uses ChannelData for id comparisons, so we clear it here
                            a.replyToId = '';
                        });
                    }

                    // DirectLine only allows the upload of at most 500 activities at a time. The limit of 1500 below is
                    // arbitrary and up to the Bot author to decide.
                    count += activities.length;
                    if (activities.length > 500 || count > 1500)
                    {
                        throw new InvalidOperationException("Attempt to upload too many activities");
                    }

                    await connector.conversations.sendConversationHistory(turnContext.activity.conversation.id, { activities });
                    continuationToken = pagedTranscript.continuationToken;
                }
                while (continuationToken != null);

                await turnContext.sendActivity("Transcript sent");
            }
            else
            {
                // Echo back to the user whatever they typed.
                await turnContext.sendActivity(`You sent: ${turnContext.activity.text}\n`);
            }
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            // Send greeting when users are added to the conversation.
            await this.sendWelcomeMessage(turnContext);
        } else {
            // Generic message for all other activities
            await turnContext.sendActivity(`[${ turnContext.activity.type } event detected]`);
        }
    }

    /**
     * Sends welcome messages to conversation members when they join the conversation.
     * Messages are only sent to conversation members who aren't the bot.
     * @param {TurnContext} turnContext
     */
    async sendWelcomeMessage(turnContext) {
        // Do we have any new members added to the conversation?
        if (turnContext.activity.membersAdded.length !== 0) {
            // Iterate over all new members added to the conversation
            for (let idx in turnContext.activity.membersAdded) {
                // Greet anyone that was not the target (recipient) of this message.
                // Since the bot is the recipient for events from the channel,
                // context.activity.membersAdded === context.activity.recipient.Id indicates the
                // bot was added to the conversation, and the opposite indicates this is a user.
                if (turnContext.activity.membersAdded[idx].id !== turnContext.activity.recipient.id) {
                    await turnContext.sendActivity(`Welcome to the 'Conversation History' Bot. This bot will introduce you to the transcript store and sending transcript activities.`);
                    await turnContext.sendActivity("You are seeing this message because the bot received at least one 'ConversationUpdate'" +
                                            'event, indicating you (and possibly others) joined the conversation. If you are using the emulator, ' +
                                            "pressing the 'Restart conversation' button to trigger this event again. The specifics of the 'ConversationUpdate' " +
                                            'event depends on the channel. You can read more information at https://aka.ms/about-botframework-welcome-user');
                    await turnContext.sendActivity(`It is a good pattern to use this event to send general greeting to user, explaining what your bot can do. ` +
                                            `In this example, the bot echo's back what the user sent, and will send Transcript History in response to '!history'` +
                                            `Try it now, type 'hi' or '!history'`);
                }
            }
        }
    }
}

module.exports.ConversationHistoryBot = ConversationHistoryBot;
