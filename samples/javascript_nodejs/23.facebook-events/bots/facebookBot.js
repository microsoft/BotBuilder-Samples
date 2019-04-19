// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');


// This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
// to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
// each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
// The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
// and the requirement is that all BotState objects are saved at the end of a turn.

class FacebookBot extends ActivityHandler {
    /**
     * Creates a FacebookEventsBot. Since we use prompts, we need to receive a conversation state instance.
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     * @param {Dialog} dialog A UserState object used to store values specific to the user.
     * @param {any} logger object for logging events, defaults to console if none is provided 
     */
    constructor(conversationState, dialog, logger) {
        super();
        if (!conversationState) throw new Error('[FacebookEventsBot]: Missing parameter. conversationState is required');
        if (!dialog) throw new Error('[FacebookEventsBot]: Missing parameter. dialog is required');
        if (!logger) {
            logger = console;
            logger.log('[FacebookEventsBot]: logger not passed in, defaulting to console');
        }

        this.conversationState = conversationState;
        this.dialog = dialog;
        this.logger = logger;
        this.dialogState = this.conversationState.createProperty('DialogState');


        this.onMessage(async (context, next) => {
            this.logger.log('Running dialog with Message Activity.');
            
            // Run the Dialog with the new message Activity.
            await this.dialog.run(context, this.dialogState);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onDialog(async (context, next) => {
            // Save any state changes. The load happened during the execution of the Dialog.
            await this.conversationState.saveChanges(context, false);
       

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onEvent(async (context, next) => {
            // Analyze Facebook payload from channel data.
            processFacebookPayload(context.activity.channelData);

            return base.onEvent(context, next);
        });
    }

}


/**
 * Process a Facebook payload from channel data, to handle optin events, postbacks and quick replies.
 * NOTE: This is a simplification of the Facebook object model. There are many more events and payloads
 * that could be captured here. We only show some key features that are commonly used. The sample
 * can be extended to account for more Facebook-specific events according to developers' needs.
 * 
 * @param {Object} channelData Channel data for the current turn.
 */

function processFacebookPayload(channelData) {
   
    // At this point we know we are on Facebook channel, and can consume the Facebook custom payload present in channelData.

    if(channelData != null)
    {
        //Postback
        if(channelData.postback != null)
         {
            onFacebookPostback(channelData.postback);
        }

        //Optin
       else if(channelData.optin != null)
       {
           onFacebookOptin(channelData.optin);
       }

       //Quick Reply
       else if (channelData.message.quickReply != null)
       {
          onFacebookQuickReply(channelData.message.quickReply);
       }
    }
}

/*function processFacebookPayload(channelData) {
    if (!channelData) {
        return;
    }

    if (channelData.postback) {
        onFacebookPostback(channelData.postback);
    } else if (channelData.optin) {
        onFacebookOptin(channelData.optin);
    } else if (channelData.message && channelData.message.quick_reply) {
        onFacebookQuickReply(channelData.message.quick_reply);
    } else {
        // TODO: Handle other events that you're interested in...
    }
}*/

/**
 * Called when receiving a Facebook messaging_postback event.
 * Facebook Developer Reference: Postback
 * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/
 * @param {Object} postback JSON object for postback payload.
 */
function onFacebookPostback(postback) {
    // TODO: Your postBack handling logic here...
}

/**
 * Called when receiving a Facebook messaging_optin event.
 * Facebook Developer Reference: Optin
 * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/
 * @param {Object} optin JSON object for postback payload.
 */
function onFacebookOptin(optin) {
    // TODO: Your optin handling logic here...
}

/**
 * Called when receiving a Facebook quick reply.
 * Facebook Developer Reference: Quick reply
 * https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/
 * @param {Object} quickReply JSON object for postback payload.
 */
function onFacebookQuickReply(quickReply) {
    // TODO: Your QuickReply handling logic here...
}

module.exports.FacebookBot = FacebookBot;
