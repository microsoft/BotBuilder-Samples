namespace CreateNewConversationBot
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using Microsoft.Bot.Builder.Dialogs;

    [Serializable]
    public sealed class SurveyScheduler : ISurveyScheduler
    {
        private readonly ConcurrentQueue<ResumptionCookie> surveyRequests = new ConcurrentQueue<ResumptionCookie>();

        public SurveyScheduler()
        {
            HostingEnvironment.QueueBackgroundWorkItem(async token =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    while (surveyRequests.Count > 0)
                    {
                        ResumptionCookie surveyRequest = null;

                        if (surveyRequests.TryDequeue(out surveyRequest))
                        {
                            await SurveyTriggerer.StartSurvey(surveyRequest, token);
                        }
                    }

                    // polling is one of the naive aspects of this implementation
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }

        public void Add(ResumptionCookie cookie)
        {
            this.surveyRequests.Enqueue(cookie);
        }
    }
}
