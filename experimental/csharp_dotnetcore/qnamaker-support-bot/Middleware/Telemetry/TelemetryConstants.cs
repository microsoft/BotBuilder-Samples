// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Middleware.Telemetry
{
    /// <summary>
    /// The Application Insights Telemetry constants to log.
    /// </summary>
    public class TelemetryConstants
    {
        public const string ReplyActivityIDProperty = "replyActivityId";
        public const string FromIdProperty = "FromId";
        public const string FromNameProperty = "FromName";
        public const string RecipientIdProperty = "recipientId";
        public const string RecipientNameProperty = "recipientName";
        public const string ConversationNameProperty = "conversationName";
        public const string TextProperty = "text";
        public const string LocaleProperty = "locale";
    }
}
