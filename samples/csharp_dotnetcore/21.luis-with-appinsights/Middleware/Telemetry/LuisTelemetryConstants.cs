// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The Application Insights property names that we're logging.
    /// </summary>
    public static class LuisTelemetryConstants
    {
        public const string IntentPrefix = "LuisIntent";  // Application Insights Custom Event name (with Intent)

        public const string IntentProperty = "intent";
        public const string IntentScoreProperty = "intent_Score";
        public const string QuestionProperty = "question";
        public const string SentimentLabelProperty = "sentiment_Label";
        public const string SentimentScoreProperty = "sentiment_Score";
        public const string DialogId = "dialog_Id";
    }
}
