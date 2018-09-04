// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TelemetryClient } from 'applicationinsights';
import { TurnContext } from 'botbuilder';
import { QnAMaker, QnAMakerEndpoint, QnAMakerOptions, QnAMakerResult } from 'botbuilder-ai';
import { EventTelemetry } from 'applicationinsights/out/Declarations/Contracts';

/**
 * Custom wrapper around QnAMaker from botbuilder-ai.
 * Sends custom events to Application Insights.
 */
export class MyAppInsightsQnAMaker extends QnAMaker {
    
    public readonly QnAMsgEvent = 'QnAMessage';

    public requestConfig: QnAMakerOptions;
    public logOriginalMessage: boolean = false;
    public logUserName: boolean = false;

    constructor(endpoint: QnAMakerEndpoint, options?: QnAMakerOptions, logOriginalMessage?: boolean, logUserName?: boolean) {
        super(endpoint);
        this.requestConfig = options;
        this.logOriginalMessage = logOriginalMessage;
        this.logUserName = logUserName;
    }

    /**
     * Calls QnA Maker and then sends a custom Event to Application Insights with the answer and question that matched closest with the user's utterance.
     * Sends the top scoring Question and Answer pairing to Application Insights.
     * @param turnContext The TurnContext instance with the necessary information to perform the calls.
     */
    public async getAnswers(turnContext: TurnContext): Promise<QnAMakerResult[]> {
        const { top, scoreThreshold } = this.requestConfig;

        // Call QnAMaker.generateAnswer to retrieve possible Question and Answer pairings for the user's utterance.
        const results: QnAMakerResult[] = await this.generateAnswer(turnContext.activity.text, top, scoreThreshold);

        // Retrieve the reference for the TelemetryClient that was cached for the Turn in TurnContext.turnState via MyAppInsightsMiddleware.
        const telemetryClient: TelemetryClient = turnContext.turnState.get('AppInsightsLoggerMiddleware.AppInsightsContext');
        const telemetryProperties: { [key: string]: string } = {};
        const telemetryMetrics: { [key: string]: number } = {};
        
        const activity = turnContext.activity;

        // Make it so we can correlate our reports with Activity or Conversation.
        telemetryProperties.ActivityId = activity.id;
        if (activity.conversation.id) {
            telemetryProperties.ConversationId = activity.conversation.id;
        }
        
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
        const qnaMsgEvent: EventTelemetry = { name: 'QnAMessage', 
                                              properties: telemetryProperties,
                                              measurements: telemetryMetrics
                                            };

        // Track the event.
        telemetryClient.trackEvent(qnaMsgEvent);

        return results;
    }
}