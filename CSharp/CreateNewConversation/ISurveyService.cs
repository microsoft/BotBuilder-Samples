namespace CreateNewConversationBot
{
    using System.Threading.Tasks;

    public interface ISurveyService
    {
        Task QueueSurveyAsync();
    }
}