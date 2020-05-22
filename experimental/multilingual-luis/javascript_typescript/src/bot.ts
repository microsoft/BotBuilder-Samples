// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityTypes } from 'botbuilder';
import { LuisRecognizer } from 'botbuilder-ai';

/**
 * A simple bot that responds to utterances with answers from the Language Understanding (LUIS) service, utilizing translation service to translate the requests to the bot native language. 
 */
export class LuisBot {
    private luisRecognizer: LuisRecognizer;
    /**
     * The LuisBot constructor requires one argument (`application`) which is used to create an instance of `LuisRecognizer`.
     * @param {LuisApplication} luisApplication The basic configuration needed to call LUIS. In this sample the configuration is retrieved from the .bot file.
     * @param {LuisPredictionOptions} luisPredictionOptions (Optional) Contains additional settings for configuring calls to LUIS.
    //  * @param {Object} userState User state object.
    //  * @param {Object} languagePreferenceProperty Accessor for language preference property in the user state.
     */

    constructor(application, luisPredictionOptions) {
        this.luisRecognizer = new LuisRecognizer(application, luisPredictionOptions, true);
    }

    /**
     * Every conversation turn calls this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {TurnContext} turnContext A TurnContext instance, containing all the data needed for processing the conversation turn.
     */
    async onTurn(turnContext) {
        // By checking the incoming Activity type, the bot only calls LUIS in appropriate cases.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Perform a call to LUIS to retrieve results for the user's message.
            const results = await this.luisRecognizer.recognize(turnContext);
            await turnContext.sendActivity(`Your input message after translation is ${results.text}`);
            
            // Since the LuisRecognizer was configured to include the raw results, get the `topScoringIntent` as specified by LUIS.
            const topIntent = results.luisResult.topScoringIntent;
            await turnContext.sendActivity(`After using LUIS recognition:\nthe top intent was:  ${ topIntent.intent }, Score: ${ topIntent.score }`);
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate &&
            turnContext.activity.recipient.id !== turnContext.activity.membersAdded[0].id) {
            // If the Activity is a ConversationUpdate, send a greeting message to the user.
            await turnContext.sendActivity('"Hello and welcome to the Luis Sample bot."');
        }
    }
}
