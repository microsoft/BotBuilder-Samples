namespace CreateNewConversationBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyScheduler surveyScheduler;
        private readonly ConversationReference conversationReference;

        public SurveyService(ISurveyScheduler surveyScheduler, ConversationReference conversationReference)
        {
            SetField.NotNull(out this.surveyScheduler, nameof(surveyScheduler), surveyScheduler);
            SetField.NotNull(out this.conversationReference, nameof(conversationReference), conversationReference);
        }

        public async Task QueueSurveyAsync()
        {
            this.surveyScheduler.Add(this.conversationReference);
        }
    }
}
