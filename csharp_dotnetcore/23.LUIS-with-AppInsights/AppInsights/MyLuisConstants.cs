// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples.AppInsights
{
    /// <summary>
    /// The Application Insights property names to log.
    /// </summary>
    public static class MyLuisConstants
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
