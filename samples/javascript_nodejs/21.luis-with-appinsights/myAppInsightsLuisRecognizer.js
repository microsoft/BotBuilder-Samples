// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { LuisRecognizer } = require('botbuilder-ai');

/**
 * Custom wrapper around LuisRecognizer from botbuilder-ai.
 * Sends custom events to Application Insights.
 */
class MyAppInsightsLuisRecognizer extends LuisRecognizer {
    constructor(application, options, includeApiResults, logOriginalMessage, logUserName) {
        super(application, options, includeApiResults);
        this.LuisMsgEvent = 'LUIS_Message';
        this.requestConfig = options;
        this.logOriginalMessage = logOriginalMessage;
        this.logUserName = logUserName;
    }

    /**
     * Calls LUIS and then sends a custom Event to Application Insights with results that matched closest with the user's message.
     * Sends the top scoring intent to Application Insights.
     * @param {TurnContext} turnContext The TurnContext instance with the necessary information to perform the calls.
     */
    async recognize(turnContext) {
        // Call LuisRecognizer.recognize to retrieve results from LUIS.
        const results = await super.recognize(turnContext);

        // Retrieve the reference for the TelemetryClient that was cached for the Turn in TurnContext.turnState via MyAppInsightsMiddleware.
        const telemetryClient = turnContext.turnState.get('AppInsightsLoggerMiddleware.AppInsightsContext');
        const telemetryProperties = {};
        const activity = turnContext.activity;

        const topLuisIntent = results.luisResult.topScoringIntent;
        const intentScore = topLuisIntent.score.toString();

        telemetryProperties.Intent = topLuisIntent.intent;
        telemetryProperties.Score = intentScore;

        // For some customers, logging original text name within Application Insights might be an issue.
        if (this.logOriginalMessage && !!activity.text) {
            telemetryProperties.OriginalMessage = activity.text;
        }

        // For some customers, logging user name within Application Insights might be an issue.
        if (this.logUserName && !!activity.from.name) {
            telemetryProperties.Username = activity.from.name;
        }

        // Finish constructing the event.
        const luisMsgEvent = { name: `LuisMessage.${ topLuisIntent.intent }`,
            properties: telemetryProperties
        };

        // Track the event.
        telemetryClient.trackEvent(luisMsgEvent);
        return results;
    }
}

module.exports.MyAppInsightsLuisRecognizer = MyAppInsightsLuisRecognizer;
