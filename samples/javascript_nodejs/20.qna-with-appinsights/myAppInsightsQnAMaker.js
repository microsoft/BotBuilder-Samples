// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { QnAMaker } = require('botbuilder-ai');

/**
 * Custom wrapper around QnAMaker from botbuilder-ai.
 * Sends custom events to Application Insights.
 */
class MyAppInsightsQnAMaker extends QnAMaker {
    constructor(endpoint, options, logOriginalMessage, logUserName) {
        super(endpoint);
        this.QnAMsgEvent = 'QnAMessage';
        this.requestConfig = options;
        this.logOriginalMessage = logOriginalMessage;
        this.logUserName = logUserName;
    }

    /**
     * Calls QnA Maker and then sends a custom Event to Application Insights with results that matched closest with the user's message.
     * Sends the top scoring Question and Answer pairing to Application Insights.
     * @param {TurnContext} turnContext The TurnContext instance with the necessary information to perform the calls.
     */
    async generateAnswer(turnContext) {
        const { top, scoreThreshold } = this.requestConfig;
        // Call QnAMaker.generateAnswer to retrieve possible Question and Answer pairings for the user's message.
        const results = await super.generateAnswer(turnContext.activity.text, top, scoreThreshold);

        // Retrieve the reference for the TelemetryClient that was cached for the Turn in TurnContext.turnState via MyAppInsightsMiddleware.
        const telemetryClient = turnContext.turnState.get('AppInsightsLoggerMiddleware.AppInsightsContext');
        const telemetryProperties = {};
        const telemetryMetrics = {};
        const activity = turnContext.activity;

        // For some customers, logging original text name within Application Insights might be an issue.
        if (this.logOriginalMessage && !!activity.text) {
            telemetryProperties.OriginalQuestion = activity.text;
        }

        // For some customers, logging user name within Application Insights might be an issue.
        if (this.logUserName && !!activity.from.name) {
            telemetryProperties.Username = activity.from.name;
        }

        // Fill in QnA Results (found or not).
        if (results.length > 0) {
            const queryResult = results[0];
            telemetryProperties.Question = queryResult.questions[0];
            telemetryProperties.Answer = queryResult.answer;
            telemetryMetrics.Score = queryResult.score;
        } else {
            telemetryProperties.Question = 'No QnA Question matched.';
            telemetryProperties.Answer = 'No QnA Question matched.';
        }

        // Finish constructing the event.
        const qnaMsgEvent = {
            name: 'QnAMessage',
            properties: telemetryProperties,
            measurements: telemetryMetrics
        };

        // Track the event.
        telemetryClient.trackEvent(qnaMsgEvent);
        return results;
    }
}

exports.MyAppInsightsQnAMaker = MyAppInsightsQnAMaker;
