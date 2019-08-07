using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class MyQnAMaker : QnAMaker
    {
        public MyQnAMaker(
            QnAMakerEndpoint endpoint,
            QnAMakerOptions options = null,
            HttpClient httpClient = null,
            IBotTelemetryClient telemetryClient = null,
            bool logPersonalInformation = false)
            : base(endpoint, options, httpClient, telemetryClient, logPersonalInformation)
        {

        }

        protected override async Task OnQnaResultsAsync(
                            QueryResult[] queryResults,
                            Microsoft.Bot.Builder.ITurnContext turnContext,
                            Dictionary<string, string> telemetryProperties = null,
                            Dictionary<string, double> telemetryMetrics = null,
                            CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventData = await FillQnAEventAsync(
                                    queryResults,
                                    turnContext,
                                    telemetryProperties,
                                    telemetryMetrics,
                                    cancellationToken)
                                .ConfigureAwait(false);

            // Add my property
            eventData.Properties.Add("MyImportantProperty", "myImportantValue");

            // Log QnaMessage event
            TelemetryClient.TrackEvent(
                            QnATelemetryConstants.QnaMsgEvent,
                            eventData.Properties,
                            eventData.Metrics
                            );

            // Create second event.
            var secondEventProperties = new Dictionary<string, string>();
            secondEventProperties.Add("MyImportantProperty2",
                                        "myImportantValue2");
            TelemetryClient.TrackEvent(
                            "MySecondEvent",
                            secondEventProperties);
        }
    }
}
