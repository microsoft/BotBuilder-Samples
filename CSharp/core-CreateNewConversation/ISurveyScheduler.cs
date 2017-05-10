namespace CreateNewConversationBot
{
    using Microsoft.Bot.Connector;

    public interface ISurveyScheduler
    {
       void Add(ConversationReference conversationReference);
    }
}