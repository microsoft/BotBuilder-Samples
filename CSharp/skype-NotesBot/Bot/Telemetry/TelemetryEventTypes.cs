namespace Bot.Telemetry
{
    public static class TelemetryEventTypes
    {
        public const string MessageBotAdded = "message.contactRelation.Added";
        public const string MessageBotRemoved = "message.contactRelation.Removed";
        public const string MessageReceived = "message.received";
        public const string MessageSend = "message.send";
        public const string MessageOthers = "message.others";
        public const string ConversationUpdate = "message.conversation.update";
        public const string ConversationEnded = "message.conversation.ended";
    }
}