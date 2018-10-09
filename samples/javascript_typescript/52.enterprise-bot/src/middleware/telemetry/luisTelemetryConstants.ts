// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

/**
 * The Application Insights property names that we're logging.
 */
export class LuisTelemetryConstants {
    public static readonly IntentPrefix: string = "LuisIntent";  // Application Insights Custom Event name (with Intent)
    public static readonly IntentProperty: string = "Intent";
    public static readonly IntentScoreProperty: string = "IntentScore";
    public static readonly ConversationIdProperty: string = "ConversationId";
    public static readonly QuestionProperty: string = "Question";
    public static readonly ActivityIdProperty: string = "ActivityId";
    public static readonly SentimentLabelProperty: string = "SentimentLabel";
    public static readonly SentimentScoreProperty: string = "SentimentScore";
}
