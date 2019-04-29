// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, TurnContext, CardFactory , ActionTypes} = require('botbuilder');
const { ChoiceFactory } = require('botbuilder-dialogs');
// These are the options provided to the user when they message the bot
const facebookPageIdOption = "Facebook Page Id";
const quickRepliesOption = "Quick Replies";
const postBackOption = "PostBack";

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
            
            // Show choices if the Facebook Payload from ChannelData is not handled
            if (!await this.processFacebookPayload(context, context.activity.channelData)) {
                if (context.activity.channelId !== "facebook") {
                    await context.sendActivity("This sample is intended to be used with a Facebook bot.");
                }
                await this.showChoices(context);
            }
        });



        this.onEvent(async (context) => {
            this.logger.log('Processing an Event Activity.');

            // Analyze Facebook payload from EventActivity.Value
            await this.processFacebookPayload(context, context.activity.value);

        });


    }

    /**
     * 
     * @param {TurnContext} context 
     */
    async showChoices(context) {
        // Create choices for the prompt
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
        
        // Create the prompt message
        var message = ChoiceFactory.forChannel(context.activity.channelId, choices, "What Facebook feature would you like to try? Here are some quick replies to choose from!");
        await context.sendActivity(message);
    };

    /**
     * @param {TurnContext} context 
     * @param {Object} data
     */

    async processFacebookPayload(context, data) {

        // At this point we know we are on Facebook channel, and can consume the Facebook custom payload present in channelData.
        const facebookPayload = data;
        if (facebookPayload) {

            //Postback
            if (facebookPayload.postback) {
                await this.onFacebookPostback(context, facebookPayload.postback);
                return true;
            }

            //Optin
            else if (facebookPayload.optin) {
                await this.onFacebookOptin(context, facebookPayload.optin);
                return true;
            }

            //Quick Reply
            else if (facebookPayload.message && facebookPayload.message.quick_reply) {
                await this.onFacebookQuickReply(context, facebookPayload.message.quick_reply);
                return true;
            }


            //Echo
            else if (facebookPayload.message && facebookPayload.message.is_echo) {
                await this.onFacebookEcho(context, facebookPayload.message);
                return true;
            }
        }
        return false;
    }

    /**
     * @param {Object} postback JSON object for postback payload.
     * @param {TurnContext} context
     */

    async onFacebookPostback(context, postback) {
        this.logger.log('Postback message received.');
        // TODO: Your postBack handling logic here...

        //Answer the postback and show choices
        await context.sendActivity("Are you sure?");
        await this.showChoices(context);
    }

    /**
     * @param {Object} optin 
     * @param {TurnContext} context
     */

    async onFacebookOptin(context, optin) {
        this.logger.log('Optin message received.')
        // TODO: Your optin handling logic here...
    }

    /**
     * @param {Object} quickReply JSON object for postback payload.
     * @param {TurnContext} context
     */

    async onFacebookQuickReply(context, quickReply) {
        this.logger.log('QuickReply message received.');
         // TODO: Your QuickReply handling logic here...

         // Process the message by checking the Activity.Text.  The FacebookQuickReply could also contain a json payload.

         // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
         switch (context.activity.text) {

            // Here we showcase how to obtain the Facebook page id.
            // This can be useful for the Facebook multi-page support provided by the Bot Framework.
            // The Facebook page id from which the message comes from is in turnContext.Activity.Recipient.Id.
            case facebookPageIdOption:

                await context.sendActivity(`This message comes from the following Facebook Page: ${context.activity.recipient.id}`);
                await this.showChoices(context);
                break;
         
            // Here we send a HeroCard with 2 options that will trigger a Facebook PostBack.
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

            // By default we offer the users different actions that the bot supports, through quick replies.
            case quickRepliesOption:
            default:
                await this.showChoices(context);
                break;

        }
    }
}

module.exports.FacebookBot = FacebookBot;

