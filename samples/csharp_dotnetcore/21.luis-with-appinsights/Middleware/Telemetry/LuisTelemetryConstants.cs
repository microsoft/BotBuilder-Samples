// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The Application Insights property names that we're logging.
    /// </summary>
    public static class LuisTelemetryConstants
    {
        public const string ApplicationIdProperty = "applicationId";
        public const string IntentPrefix = "LuisResult";
        public const string IntentProperty = "intent";
        public const string IntentScoreProperty = "intentScore";
        public const string EntitiesProperty = "entities";
        public const string QuestionProperty = "question";
        public const string ActivityIdProperty = "activityId";
        public const string SentimentLabelProperty = "sentimentLabel";
        public const string SentimentScoreProperty = "sentimentScore";
        public const string FromIdProperty = "fromId";
    }
}
