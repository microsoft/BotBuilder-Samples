// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, TurnContext } = require('botbuilder');
const { ChoiceFactory } = require('botbuilder-dialogs');
const facebookPageIdOption = "Facebook Page Id";
const quickRepliesOption = "Quick Replies";
const postBackOption = "PostBack";

// This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
// to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
// each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
// The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
// and the requirement is that all BotState objects are saved at the end of a turn.

class FacebookBot extends ActivityHandler {

    /**
     * @param {any} logger object for logging events, defaults to console if none is provided 
     */
    constructor(logger) {
        super();
        if (!logger) {
            logger = console;
            logger.log('[FacebookEventsBot]: logger not passed in, defaulting to console');
        }


        this.logger = logger;



        this.onMessage(async (context) => {
            this.logger.log('Processing a Message Activity.');

            if (!await this.processFacebookPayload(context, context.activity.channelData)) {
                await this.showChoices(context);
            }
        });



        this.onEvent(async (context) => {
            this.logger.log('Processing an Event Activity.');

            // Analyze Facebook payload from channel data.
            await this.processFacebookPayload(context, context.activity.value);

        });

        
    }

    /**
     * 
     * @param {TurnContext} context 
     */
    async showChoices(context) 
    {
       
          const choices = [
                {
                    value: quickRepliesOption
                },
                {
                    value: facebookPageIdOption
                },
                {
                    value: postBackOption
                }
            ]
        
        var message = ChoiceFactory.forChannel(context.activity.channelId, choices, "What Facebook feature would you like to try? Here are some quick replies to choose from!");
        await context.sendActivity(message);
    };
    /**
 * Process a Facebook payload from channel data, to handle optin events, postbacks and quick replies.
 * NOTE: This is a simplification of the Facebook object model. There are many more events and payloads
 * that could be captured here. We only show some key features that are commonly used. The sample
 * can be extended to account for more Facebook-specific events according to developers' needs.
 * 
 */

    async processFacebookPayload(context, data) {

        // At this point we know we are on Facebook channel, and can consume the Facebook custom payload present in channelData.
        const facebookPayload = data;
        if (facebookPayload != null) {
            //Postback
            if (facebookPayload.postback != null) {
                await this.onFacebookPostback(context, facebookPayload.postback);
                return true;
            }

            //Optin
            else if (facebookPayload.optin != null) {
                await this.onFacebookOptin(context, facebookPayload.optin);
                return true;
            }

            //Quick Reply
            else if (facebookPayload.message != null && facebookPayload.message.quickReply != null) {
                await this.onFacebookQuickReply(context, facebookPayload.message.quickReply);
                return true;
            }


            //Echo
            else if (facebookPayload.message != null && facebookPayload.message.isEcho != null && facebookPayload.message.isEcho) {
                await this.onFacebookEcho(context, facebookPayload.message);
                return true;
            }
        }
        return false;
    }

    /**
     * Called when receiving a Facebook messaging_postback event.
     * Facebook Developer Reference: Postback
     * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/
     * @param {Object} postback JSON object for postback payload.
     */
    async onFacebookPostback(context, postback) {
        this.logger.log('Postback message received.');
        // TODO: Your postBack handling logic here...

        //Answer the postback and show choices
        await context.sendActivity("ARe you sure?");
        await this.showChoices(context);
    }

    /**
     * Called when receiving a Facebook messaging_optin event.
     * Facebook Developer Reference: Optin
     * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/
     * @param {Object} optin JSON object for postback payload.
     */
    async onFacebookOptin(context, optin) {
        this.logger.log('Optin message received.')
        // TODO: Your optin handling logic here...
    }

    /**
     * Called when receiving a Facebook quick reply.
     * Facebook Developer Reference: Quick reply
     * https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/
     * @param {Object} quickReply JSON object for postback payload.
     */
    async onFacebookQuickReply(context, quickReply) {
        this.logger.log('QuickReply message received.');
        // TODO: Your QuickReply handling logic here...


        switch (context.activity.text) {

            // Here we showcase how to obtain the Facebook page id.
            // This can be useful for the Facebook multi-page support provided by the Bot Framework.
            // The Facebook page id from which the message comes from is in turnContext.Activity.Recipient.Id.
            case facebookPageIdOption:

                await context.sendActivity(`This message comes from the following Facebook Page: ${context.activity.recipient.id}`);
                await this.showChoices(context);
                break;

            case postBackOption:
                var card = CardFactory.heroCard(
                    'Is 42 the answer to the ultimate question of Life, the Universe, and Everything?',
                    null,
                    CardFactory.actions([
                        {
                            type: ActionTypes.PostBack,
                            title: 'Yes',
                            value: 'Yes'
                        },

                        {
                            type: ActionTypes.PostBack,
                            title: 'No',
                            value: 'No'
                        }
                    ])
                );

                var reply = {
                    attachments: [
                        card
                    ]
                };
                await context.sendActivity(reply);
                break;

            case quickRepliesOption:
            default:
                await this.showChoices(context);
                break;

        }
    }
}



module.exports.FacebookBot = FacebookBot;