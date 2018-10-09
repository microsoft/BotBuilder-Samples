// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { TelemetryClient } from "applicationinsights";
import { RecognizerResult, TurnContext } from "botbuilder";
import { LuisApplication, LuisPredictionOptions, LuisRecognizer } from "botbuilder-ai";
import { LuisTelemetryConstants } from "./luisTelemetryConstants";
import { TelemetryLoggerMiddleware } from "./telemetryLoggerMiddleware";

/**
 * TelemetryLuisRecognizer invokes the Luis Recognizer and logs some results into Application Insights.
 * Logs the Top Intent, Sentiment (label/score), (Optionally) Original Text Along with Conversation
 * and ActivityID.
 * The Custom Event name this logs is MyLuisConstants.IntentPrefix + "." + 'found intent name'
 * For example, if intent name was "add_calender": LuisIntent.add_calendar
 */
export class TelemetryLuisRecognizer extends LuisRecognizer {
    private readonly _logOriginalMessage: boolean;
    private readonly _logUsername: boolean;

    /**
     * Initializes a new instance of the TelemetryLuisRecognizer class.
     * @param {LuisApplication} application The LUIS application to use to recognize text.
     * @param {LuisPredictionOptions} predictionOptions The LUIS prediction options to use.
     * @param {boolean} includeApiResults TRUE to include raw LUIS API response.
     * @param {boolean} logOriginalMessage TRUE to include original user message.
     * @param {boolean} logUserName TRUE to include user name.
     */
    constructor(application: LuisApplication, predictionOptions?: LuisPredictionOptions, includeApiResults: boolean = false, logOriginalMessage: boolean = false, logUserName: boolean = false) {
        super(application, predictionOptions, includeApiResults);
        this._logOriginalMessage = logOriginalMessage;
        this._logUsername = logUserName;
    }

    /**
     * Gets a value indicating whether determines whether to log the Activity message text that came from the user.
     */
    public get logOriginalMessage(): boolean { return this._logOriginalMessage; }

    /**
     * Gets a value indicating whether determines whether to log the User name.
     */
    public get logUsername(): boolean { return this._logUsername; }

    /**
     * Analyze the current message text and return results of the analysis (Suggested actions and intents).
     * @param {TurnContext} context Context object containing information for a single turn of conversation with a user.
     * @param {boolean} logOriginalMessage Determines if the original message is logged into Application Insights. This is a privacy consideration.
     */
    public async recognize(context: TurnContext, logOriginalMessage: boolean = false): Promise<RecognizerResult> {
        if (context === null) {
            throw new Error("context is null");
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
            properties[LuisTelemetryConstants.ActivityIdProperty] = context.activity.id || "";
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
                properties,
            });
        }

        return recognizerResult;
    }
}
