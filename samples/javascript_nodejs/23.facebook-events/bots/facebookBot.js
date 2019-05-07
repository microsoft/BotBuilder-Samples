// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, CardFactory, ActionTypes } = require('botbuilder');
const { ChoiceFactory } = require('botbuilder-dialogs');
// These are the options provided to the user when they message the bot
const facebookPageIdOption = 'Facebook Page Id';
const quickRepliesOption = 'Quick Replies';
const postBackOption = 'PostBack';

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

        this.onMessage(async (turnContext) => {
            this.logger.log('Processing a Message Activity.');

            // Show choices if the Facebook Payload from ChannelData is not handled
            if (!await this.processFacebookPayload(turnContext, turnContext.activity.channelData)) {
                if (turnContext.activity.channelId !== 'facebook') {
                    await turnContext.sendActivity('This sample is intended to be used with a Facebook bot.');
                }
                await this.showChoices(turnContext);
            }
        });

        this.onEvent(async (turnContext) => {
            this.logger.log('Processing an Event Activity.');

            // Analyze Facebook payload from EventActivity.Value
            await this.processFacebookPayload(turnContext, turnContext.activity.value);
        });
    }

    /**
     * Shows a list of Facebook features for the user to choose from
     * @param {Object} turnContext
     */
    async showChoices(turnContext) {
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
        ];

        // Create the prompt message
        var message = ChoiceFactory.forChannel(turnContext.activity.channelId, choices, 'What Facebook feature would you like to try? Here are some quick replies to choose from!');
        await turnContext.sendActivity(message);
    };

    /**
     * Process a Facebook payload from channel data, to handle optin events, postbacks and quick replies.
     * NOTE: This is a simplification of the Facebook object model. There are many more events and payloads
     * that could be captured here. We only show some key features that are commonly used. The sample
     * can be extended to account for more Facebook-specific events according to developers' needs.
     * @param {Object} turnContext
     * @param {Object} data
     */
    async processFacebookPayload(turnContext, data) {
        // At this point we know we are on Facebook channel, and can consume the Facebook custom payload present in channelData.
        const facebookPayload = data;
        if (facebookPayload) {
            if (facebookPayload.postback) {
                // Postback
                await this.onFacebookPostback(turnContext, facebookPayload.postback);
                return true;
            } else if (facebookPayload.optin) {
                // Optin
                await this.onFacebookOptin(turnContext, facebookPayload.optin);
                return true;
            } else if (facebookPayload.message && facebookPayload.message.quick_reply) {
                // Quick Reply
                await this.onFacebookQuickReply(turnContext, facebookPayload.message.quick_reply);
                return true;
            } else if (facebookPayload.message && facebookPayload.message.is_echo) {
                // Echo
                await this.onFacebookEcho(turnContext, facebookPayload.message);
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
     * @param {Object} turnContext
     */
    async onFacebookPostback(turnContext, postback) {
        this.logger.log('Postback message received.');
        // TODO: Your postBack handling logic here...

        // Answer the postback and show choices
        await turnContext.sendActivity('Are you sure?');
        await this.showChoices(turnContext);
    }

    /**
     * Called when a message has been sent by your Facebook page.
     * Facebook Developer Reference: Echoes
     * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/message-echoes/
     * @param {Object} facebookMessage
     * @param {Object} turnContext
     */
    async onFacebookEcho(turnContext, facebookMessage) {
        this.logger.log('Echo message received.');
    }

    /**
     * Called when receiving a Facebook messaging_optin event.
     * Facebook Developer Reference: Optin
     * https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/
     * @param {Object} optin
     * @param {Object} turnContext
     */
    async onFacebookOptin(turnContext, optin) {
        this.logger.log('Optin message received.');
        // TODO: Your optin handling logic here...
    }

    /**
     * Called when receiving a Facebook quick reply.
     * Facebook Developer Reference: Quick reply
     * https://developers.facebook.com/docs/messenger-platform/send-messages/quick-replies/
     * @param {Object} quickReply
     * @param {Object} turnContext
     */
    async onFacebookQuickReply(turnContext, quickReply) {
        this.logger.log('QuickReply message received.');
        // TODO: Your QuickReply handling logic here...

        // Process the message by checking the Activity.Text.  The FacebookQuickReply could also contain a json payload.

        // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
        switch (turnContext.activity.text) {
        // Here we showcase how to obtain the Facebook page id.
        // This can be useful for the Facebook multi-page support provided by the Bot Framework.
        // The Facebook page id from which the message comes from is in turnContext.Activity.Recipient.Id.
        case facebookPageIdOption:

            await turnContext.sendActivity(`This message comes from the following Facebook Page: ${ turnContext.activity.recipient.id }`);
            await this.showChoices(turnContext);
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
            await turnContext.sendActivity(reply);
            break;

        // By default we offer the users different actions that the bot supports, through quick replies.
        case quickRepliesOption:
        default:
            await this.showChoices(turnContext);
            break;
        }
    }
}

module.exports.FacebookBot = FacebookBot;
