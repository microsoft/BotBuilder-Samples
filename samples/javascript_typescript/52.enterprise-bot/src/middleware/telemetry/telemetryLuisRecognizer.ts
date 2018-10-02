// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { LuisRecognizer, LuisApplication, LuisPredictionOptions } from 'botbuilder-ai';
import { RecognizerResult, TurnContext } from 'botbuilder';
import { TelemetryLoggerMiddleware } from './telemetryLoggerMiddleware';
import { TelemetryClient } from 'applicationinsights';
import { LuisTelemetryConstants } from './luisTelemetryConstants';

export class TelemetryLuisRecognizer extends LuisRecognizer {
    private readonly _logOriginalMessage: boolean;
    private readonly _logUsername: boolean;

    /**
     *
     */
    constructor(application: LuisApplication, predictionOptions?: LuisPredictionOptions, includeApiResults: boolean = false, logOriginalMessage: boolean = false, logUserName: boolean = false) {
        super(application, predictionOptions, includeApiResults);
        this._logOriginalMessage = logOriginalMessage;
        this._logUsername = logUserName;    
    }

    public get logOriginalMessage(): boolean { return this._logOriginalMessage; }

    public get logUsername(): boolean { return this._logUsername; }

    public async recognize(context: TurnContext, logOriginalMessage: boolean = false): Promise<RecognizerResult> {
        if (context === null) {
            throw new Error('context is null');
        }

        // Call Luis Recognizer
        const recognizerResult: RecognizerResult = await super.recognize(context);

        const conversationId: string = context.activity.conversation.id;

        // Find the Telemetry Client
        if (recognizerResult && context.turnState.has(TelemetryLoggerMiddleware.AppInsightsServiceKey)) {
            const telemetryClient: TelemetryClient = context.turnState.get(TelemetryLoggerMiddleware.AppInsightsServiceKey);

            const topLuisIntent: string = LuisRecognizer.topIntent(recognizerResult);
            const intentScore: number = recognizerResult.intents[topLuisIntent].score;

            // Add the intent score and conversation id properties
            const properties: { [key: string]: string } = {};
            properties[LuisTelemetryConstants.ActivityIdProperty] = context.activity.id || '';
            properties[LuisTelemetryConstants.IntentProperty] = topLuisIntent;
            properties[LuisTelemetryConstants.IntentScoreProperty] = intentScore.toString();

            if (recognizerResult.sentiment) {
                if (recognizerResult.sentiment.label) {
                    properties[LuisTelemetryConstants.SentimentLabelProperty] = recognizerResult.sentiment.label;
                }
                
                if (recognizerResult.sentiment.score) {
                    properties[LuisTelemetryConstants.SentimentScoreProperty] = recognizerResult.sentiment.score.toString();
                }
            }

            if (conversationId) {
                properties[LuisTelemetryConstants.ConversationIdProperty] = conversationId;
            }

            // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
            if (logOriginalMessage && context.activity.text) {
                properties[LuisTelemetryConstants.QuestionProperty] = context.activity.text;
            }

            // Track the event
            telemetryClient.trackEvent({
                name: `${LuisTelemetryConstants.IntentPrefix}.${topLuisIntent}`,
                properties: properties
            });
        }

        return recognizerResult;
    }
}