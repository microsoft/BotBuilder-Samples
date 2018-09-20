// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory } = require('botbuilder');
const { ChoicePrompt, DialogSet } = require('botbuilder-dialogs');

const DIALOG_STATE_PROPERTY = 'dialogState';
const SAMPLE_PROMPT = 'sample_prompt';

/**
 * A simple bot that responds handles and shows Facebook events.
 */
class FacebookEventsBot {
    /**
     * Creates a FacebookEventsBot. Since we use prompts, we need to receive a conversation state instance.
     * @param {ConversationState} conversationState A ConversationState object used to store the dialog state.
     * @param {UserState} userState A UserState object used to store values specific to the user.
     */
    constructor(conversationState, userState) {
        this.conversationState = conversationState;
        this.userState = userState;

        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        this.dialogs = new DialogSet(this.dialogState);
        this.dialogs.add(new ChoicePrompt(SAMPLE_PROMPT));
    }

    /**
     * Every conversation turn for our FacebookEventsBot will call this method.
     * @param {Object} turnContext on turn context object.
     */
    async onTurn(turnContext) {
        // These are the 3 main Facebook features we show in this sample
        const facebookPageNameOption = 'Facebook Page Name';
        const quickRepliesOption = 'Quick Replies';
        const postBackOption = 'PostBack';

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;

            // Check if we are on the Facebook channel.
            if (turnContext.activity.channelId === 'facebook') {
                // Analyze Facebook payload from channel data.
                processFacebookPayload(turnContext.activity.channelData);

                // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
                // Below we also show how to get the messaging_optin payload as well.
                switch (text) {
                // Here we showcase how to obtain the Facebook page name.
                // This can be useful for the Facebook multi-page support provided by the Bot Framework.
                // The Facebook page name from which the message comes from is in turnContext.activity.recipient.name.
                case facebookPageNameOption:
                    await turnContext.sendActivity(`This message comes from the following Facebook Page: ${ turnContext.activity.recipient.name }`);
                    break;
                case postBackOption:
                    // Construct a DialogContext instance which is used to resume any
                    // existing Dialogs and prompt users.
                    const dc = await this.dialogs.createContext(turnContext);

                    await dc.continueDialog();

                    // Create the PromptOptions which contain the prompt and reprompt messages.
                    // PromptOptions also contains the list of choices available to the user.
                    const promptOptions = {
                        prompt: 'Is 42 the answer to the ultimate question of Life, the Universe, and Everything?',
                        reprompt: 'Please answer Yes or No.',
                        choices: [{ value: 'Yes' }, { value: 'No' }] };

                    // Prompt the user with the configured PromptOptions.
                    await dc.prompt(SAMPLE_PROMPT, promptOptions);
                    break;

                case quickRepliesOption:
                default:
                    var reply = MessageFactory.suggestedActions(
                        [facebookPageNameOption, postBackOption, quickRepliesOption],
                        `What Facebook feature would you like to try? Here are some quick replies to choose from!`);
                    await turnContext.sendActivity(reply);
                }
            } else {
                // This message didn't come from Facebook. Make sure the user knows this bot works on FB only.
                await turnContext.sendActivity(`I am a Facebook bot. To test me, send me a message with Facebook Messenger.`);
            }
        } else {
            await turnContext.sendActivity(`[${ turnContext.activity.type } event detected.]`);

            // Check if we are on the Facebook channel.
            if (turnContext.activity.channelId === 'facebook') {
                // Analyze Facebook payload from channel data to trigger custom events.
                processFacebookPayload(turnContext.activity.channelData);
            }
        }
        await this.conversationState.saveChanges(turnContext);
    }
}

/**
 * Process a Facebook payload from channel data, to handle optin events, postbacks and quick replies.
 * NOTE: This is a simplification of the Facebook object model. There are many more events and payloads
 * that could be captured here. We only show some key features that are commonly used. The sample
 * can be extended to account for more Facebook-specific events according to developers' needs.
 * @param {Object} channelData Channel data for the current turn.
 */
function processFacebookPayload(channelData) {
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
}

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

module.exports = FacebookEventsBot;
