// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { TelemetryClient } from "applicationinsights";
import { TurnContext } from "botbuilder";
import { QnAMaker, QnAMakerEndpoint, QnAMakerOptions, QnAMakerResult } from "botbuilder-ai";
import { QnATelemetryConstants } from "./qnaTelemetryConstants";
import { TelemetryLoggerMiddleware } from "./telemetryLoggerMiddleware";

/**
 * TelemetryQnaRecognizer invokes the Qna Maker and logs some results into Application Insights.
 * Logs the score, and (optionally) questionAlong with Conversation and ActivityID.
 * The Custom Event name this logs is "QnaMessage"
 */
export class TelemetryQnAMaker extends QnAMaker {
    public static readonly QnAMessageEvent: string = "QnaMessage";

    private readonly _logOriginalMessage: boolean;
    private readonly _logUserName: boolean;
    private _options: { top: number, scoreThreshold: number } = { top: 1, scoreThreshold: 0.3 };

    /**
     * Initializes a new instance of the TelemetryQnAMaker class.
     * @param {QnAMakerEndpoint} endpoint The endpoint of the knowledge base to query.
     * @param {QnAMakerOptions} options The options for the QnA Maker knowledge base.
     * @param {boolean} logUserName The flag to include username in logs.
     * @param {boolean} logOriginalMessage The flag to include original message in logs.
     */
    constructor(endpoint: QnAMakerEndpoint, options?: QnAMakerOptions, logUserName: boolean = false, logOriginalMessage: boolean = false) {
        super(endpoint, options);
        this._logOriginalMessage = logOriginalMessage;
        this._logUserName = logUserName;
        Object.assign(this._options, options);
    }

    /**
     * Gets a value indicating whether determines whether to log the Activity message text that came from the user.
     */
    public get logOriginalMessage(): boolean { return this._logOriginalMessage; }

    /**
     * Gets a value indicating whether determines whether to log the User name.
     */
    public get logUserName(): boolean { return this._logUserName; }

    public async getAnswersAsync(context: TurnContext): Promise<QnAMakerResult[]> {
        // Call Qna Maker
        const queryResults: QnAMakerResult[] = await super.generateAnswer(context.activity.text, this._options.top, this._options.scoreThreshold);

        // Find the Application Insights Telemetry Client
        if (queryResults && context.turnState.has(TelemetryLoggerMiddleware.AppInsightsServiceKey)) {
            const telemetryClient: TelemetryClient = context.turnState.get(TelemetryLoggerMiddleware.AppInsightsServiceKey);

            const properties: { [key: string]: string } = {};
            const metrics: { [key: string]: number } = {};

            // Make it so we can correlate our reports with Activity or Conversation
            properties[QnATelemetryConstants.ActivityIdProperty] = context.activity.id || "";
            const conversationId: string = context.activity.conversation.id;
            if (conversationId) {
                properties[QnATelemetryConstants.ConversationIdProperty] = conversationId;
            }

            // For some customers, logging original text name within Application Insights might be an issue
            const text: string = context.activity.text;
            if (this.logOriginalMessage && text) {
                properties[QnATelemetryConstants.OriginalQuestionProperty] = text;
            }

            // For some customers, logging user name within Application Insights might be an issue
            const name: string = context.activity.from.name;
            if (this.logUserName && name) {
                properties[QnATelemetryConstants.UsernameProperty] = name;
            }

            // Fill in Qna Results (found or not)
            if (queryResults.length > 0) {
                const queryResult: QnAMakerResult = queryResults[0];
                properties[QnATelemetryConstants.QuestionProperty] = Array.of(queryResult.questions).join(",");
                properties[QnATelemetryConstants.AnswerProperty] = queryResult.answer;
                metrics[QnATelemetryConstants.ScoreProperty] = queryResult.score;
            } else {
                properties[QnATelemetryConstants.QuestionProperty] = "No Qna Question matched";
                properties[QnATelemetryConstants.AnswerProperty] = "No Qna Question matched";
            }

            // Track the event
            telemetryClient.trackEvent({
                measurements: metrics,
                name: TelemetryQnAMaker.QnAMessageEvent,
                properties,
            });
        }

        return queryResults;
    }
}
