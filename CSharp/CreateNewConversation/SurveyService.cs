namespace CreateNewConversationBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;

    [Serializable]
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyScheduler surveyScheduler;
        private readonly ResumptionCookie cookie;

        public SurveyService(ISurveyScheduler surveyScheduler, ResumptionCookie cookie)
        {
            SetField.NotNull(out this.surveyScheduler, nameof(surveyScheduler), surveyScheduler);
            SetField.NotNull(out this.cookie, nameof(cookie), cookie);
        }

        public async Task QueueSurveyAsync()
        {
            this.surveyScheduler.Add(this.cookie);
        }
    }
}
