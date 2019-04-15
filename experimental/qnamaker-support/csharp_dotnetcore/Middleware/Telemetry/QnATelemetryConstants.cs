// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Middleware.Telemetry
{
    /// <summary>
    /// The Application Insights property names to log.
    /// </summary>
    public class QnATelemetryConstants
    {
        public const string UsernameProperty = "username";
        public const string OriginalQuestionProperty = "originalQuestion";
        public const string OriginalMetadataProperty = "originalMetadata";
        public const string QuestionProperty = "question";
        public const string AnswerProperty = "answer";
        public const string ScoreProperty = "score";
        public const string MetadataProperty = "metadata";
    }
}
