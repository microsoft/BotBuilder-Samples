// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');
const { ActivityTypes } = require('botbuilder');
const { HomeAutomation } = require('./homeAutomation');
const { Weather } = require('./weather');
const { QnA } = require('./qna');

// this is the LUIS service type entry in the .bot file.
const DISPATCH_CONFIG = 'nlp-with-dispatchDispatch';

// LUIS intent names. you can get this from the dispatch.lu file.
const HOME_AUTOMATION_INTENT = 'l_Home_Automation';
const WEATHER_INTENT = 'l_Weather';
const NONE_INTENT = 'None';
const QNA_INTENT = 'q_sample-qna';

class DispatchBot {
    /**
     *
     * @param {ConversationState}  conversation state
     * @param {UserState} user state
     * @param {BotConfiguration} bot configuration from .bot file
     */
    constructor(conversationState, userState, botConfig) {
        if (!conversationState) throw new Error(`Missing parameter. Conversation state parameter is missing`);
        if (!userState) throw new Error(`Missing parameter. User state parameter is missing`);
        if (!botConfig) throw new Error(`Missing parameter. Bot configuration parameter is missing`);

        this.homeAutomationDialog = new HomeAutomation(conversationState, userState, botConfig);
        this.weatherDialog = new Weather(botConfig);
        this.qnaDialog = new QnA(botConfig);

        this.conversationState = conversationState;
        this.userState = userState;

        // dispatch recognizer
        const dispatchServiceName = botConfig.name + '_' + DISPATCH_CONFIG;
        const dispatchConfig = botConfig.findServiceByNameOrId(dispatchServiceName);
        if (!dispatchConfig || !dispatchConfig.appId) throw new Error(`No dispatch model found in .bot file. Please ensure you have dispatch model created and available in the .bot file. See readme.md for additional information\n`);
        this.dispatchRecognizer = new LuisRecognizer({
            applicationId: dispatchConfig.appId,
            endpoint: dispatchConfig.getEndpoint(),
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: dispatchConfig.authoringKey
        });
    }

    /**
     * Driver code that does one of the following:
     * 1. Calls dispatch LUIS model to determine intent
     * 2. Calls appropriate sub component to drive the conversation forward.
     *
     * @param {TurnContext} context turn context from the adapter
     */
    async onTurn(turnContext) {
        if (turnContext.activity.type === ActivityTypes.Message) {
            // determine which dialog should fulfill this request
            // call the dispatch LUIS model to get results.
            const dispatchResults = await this.dispatchRecognizer.recognize(turnContext);
            const dispatchTopIntent = LuisRecognizer.topIntent(dispatchResults);
            switch (dispatchTopIntent) {
            case HOME_AUTOMATION_INTENT:
                await this.homeAutomationDialog.onTurn(turnContext);
                break;
            case WEATHER_INTENT:
                await this.weatherDialog.onTurn(turnContext);
                break;
            case QNA_INTENT:
                await this.qnaDialog.onTurn(turnContext);
                break;
            case NONE_INTENT:
            default:
                // Unknown request
                await turnContext.sendActivity(`I do not understand that.`);
                await turnContext.sendActivity(`I can help with weather forecast, turning devices on and off and answer general questions like 'hi', 'who are you' etc.`);
            }

            // save state changes
            await this.conversationState.saveChanges(turnContext);
            await this.userState.saveChanges(turnContext);
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            // Handle ConversationUpdate activity type, which is used to indicates new members add to
            // the conversation.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types

            // Do we have any new members added to the conversation?
            if (turnContext.activity.membersAdded.length !== 0) {
                // Iterate over all new members added to the conversation
                for (var idx in turnContext.activity.membersAdded) {
                    // Greet anyone that was not the target (recipient) of this message
                    // the 'bot' is the recipient for events from the channel,
                    // turnContext.activity.membersAdded == turnContext.activity.recipient.Id indicates the
                    // bot was added to the conversation.
                    if (turnContext.activity.membersAdded[idx].id !== turnContext.activity.recipient.id) {
                        // Welcome user.
                        // When activity type is "conversationUpdate" and the member joining the conversation is the bot
                        // we will send our Welcome Adaptive Card.  This will only be sent once, when the Bot joins conversation
                        // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                        await turnContext.sendActivity(`Hello, I am the NLP Dispatch bot!`);
                        await turnContext.sendActivity(`I can help with weather forecast, turning devices on and off and answer general questions like 'hi', 'who are you' etc.`);
                    }
                }
            }
        }
    }
}

module.exports.DispatchBot = DispatchBot;
