namespace CreateNewConversationBot
{
    using Microsoft.Bot.Builder.Dialogs;

    public interface ISurveyScheduler
    {
       void Add(ResumptionCookie cookie);
    }
}