// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace EnterpriseBot
{
    /// <summary>
    /// The Application Insights property names that we're logging.
    /// </summary>
    public static class LuisTelemetryConstants
    {
        public const string IntentPrefix = "LuisIntent";  // Application Insights Custom Event name (with Intent)
        public const string IntentProperty = "Intent";
        public const string IntentScoreProperty = "IntentScore";
        public const string ConversationIdProperty = "ConversationId";
        public const string QuestionProperty = "Question";
        public const string ActivityIdProperty = "ActivityId";
        public const string SentimentLabelProperty = "SentimentLabel";
        public const string SentimentScoreProperty = "SentimentScore";
    }
}
