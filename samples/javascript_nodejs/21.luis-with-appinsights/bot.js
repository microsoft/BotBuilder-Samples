// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { MyAppInsightsLuisRecognizer } = require('./myAppInsightsLuisRecognizer');

/**
 * A simple LUIS bot that responds to queries and uses custom middleware to send telemetry data to Application Insights.
 */
class LuisBot {
    /**
     * The LuisBot constructor requires one argument (`application`) which is used to create a MyAppInsightsLuisRecognizer singleton.
     * @param application In this sample the LUIS application's configuration is retrieved from the .bot file.
     * @param options An optional parameter that contains additional settings for configuring a LUIS calls at runtime.
     * @param includeApiResults A boolean indicating if the raw results from LUIS should be returned from MyAppInsightsLuisRecognizer.
     * @param logOriginalMessage A boolean indicating if the bot should send the user's utterance to Application Insights with the LUIS information.
     * @param logUserName A boolean indicating if the bot should send the username to Application Insights with the LUIS information.
     */
    constructor(application, options = {}, includeApiResults = false, logOriginalMessage = false, logUserName = false) {
        this.luisRecognizer = new MyAppInsightsLuisRecognizer(application, options, includeApiResults, logOriginalMessage, logUserName);
    }
    /**
     * Every conversation turn for our LuisBot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single
     * request and response, with no stateful conversation.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async onTurn(turnContext) {
        // By checking the incoming activity's type, the bot only calls LUIS in appropriate cases.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Perform a call to LUIS to retrieve results for the user's message.
            const results = await this.luisRecognizer.recognize(turnContext);

            // Since the LuisRecognizer was configured to include the raw results, get the `topScoringIntent` as specified by LUIS.
            const topIntent = results.luisResult.topScoringIntent;

            if (topIntent.intent !== 'None') {
                await turnContext.sendActivity(`LUIS Top Scoring Intent: ${ topIntent.intent }, Score: ${ topIntent.score }`);
            } else {
                // If the top scoring intent was "None" tell the user no valid intents were found and provide help.
                await turnContext.sendActivity(`No LUIS intents were found.
                                                \nThis sample is about identifying two user intents:
                                                \n - 'Calendar.Add'
                                                \n - 'Calendar.Find'
                                                \nTry typing 'Add Event' or 'Show me tomorrow'.`);
            }
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate &&
            turnContext.activity.recipient.id !== turnContext.activity.membersAdded[0].id) {
            // If the Activity is a ConversationUpdate, send a greeting message to the user.
            await turnContext.sendActivity('Welcome to the NLP with LUIS sample! Send me a message and I will try to predict your intent.');
        } else if (turnContext.activity.type !== ActivityTypes.ConversationUpdate) {
            // Respond to all other Activity types.
            await turnContext.sendActivity(`[${ turnContext.activity.type }]-type activity detected.`);
        }
    }
}

module.exports.LuisBot = LuisBot;
