// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// MyAppInsightsQnARecognizer invokes the QnA Maker and logs some results into Application Insights.
    /// Logs the score, and (optionally) question along with Conversation and ActivityID.
    ///
    /// Customize for specific reporting needs.
    ///
    /// The Custom Event name this logs is "QnAMessage"
    /// See <seealso cref="QnAMaker"/> for additional information.
    /// </summary>
    public class TelemetryQnaMaker : QnAMaker
    {
        public static readonly string QnAMsgEvent = "QnAMessage";

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryQnaMaker"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint of the knowledge base to query.</param>
        /// <param name="options">The options for the QnA Maker knowledge base.</param>
        /// <param name="logUserName">Option to log user name to Application Insights (PII consideration).</param>
        /// <param name="logOriginalMessage">Option to log the original message to Application Insights (PII consideration).</param>
        /// <param name="httpClient">An alternate client with which to talk to QnAMaker.
        /// If null, a default client is used for this instance.</param>
        public TelemetryQnaMaker(QnAMakerEndpoint endpoint, QnAMakerOptions options = null, bool logUserName = false, bool logOriginalMessage = false, HttpClient httpClient = null)
            : base(endpoint, options, httpClient)
        {
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
        }

        public bool LogUserName { get; }

        public bool LogOriginalMessage { get; }

        public new async Task<QueryResult[]> GetAnswersAsync(ITurnContext context, QnAMakerOptions options = null)
        {
            // Call QnA Maker
            var queryResults = await base.GetAnswersAsync(context, options);

            // Find the Bot Telemetry Client
            if (queryResults != null && context.TurnState.TryGetValue(TelemetryLoggerMiddleware.AppInsightsServiceKey, out var telemetryClient))
            {
                var telemetryProperties = new Dictionary<string, string>();
                var telemetryMetrics = new Dictionary<string, double>();

                // For some customers, logging original text name within Application Insights might be an issue.
                var text = context.Activity.Text;
                if (LogOriginalMessage && !string.IsNullOrWhiteSpace(text))
                {
                    telemetryProperties.Add(QnATelemetryConstants.OriginalQuestionProperty, text);
                }

                // For some customers, logging user name within Application Insights might be an issue.
                var userName = context.Activity.From.Name;
                if (LogUserName && !string.IsNullOrWhiteSpace(userName))
                {
                    telemetryProperties.Add(QnATelemetryConstants.UsernameProperty, userName);
                }

                // Fill in Qna Results (found or not)
                if (queryResults.Length > 0)
                {
                    var queryResult = queryResults[0];
                    telemetryProperties.Add(QnATelemetryConstants.QuestionProperty, string.Join(",", queryResult.Questions));
                    telemetryProperties.Add(QnATelemetryConstants.AnswerProperty, queryResult.Answer);
                    telemetryMetrics.Add(QnATelemetryConstants.ScoreProperty, (double)queryResult.Score);
                }
                else
                {
                    telemetryProperties.Add(QnATelemetryConstants.QuestionProperty, "No Qna Question matched");
                    telemetryProperties.Add(QnATelemetryConstants.AnswerProperty, "No Qna Question matched");
                }

                // Track the event
                ((IBotTelemetryClient)telemetryClient).TrackEvent(QnAMsgEvent, telemetryProperties, telemetryMetrics);
            }

            return queryResults;
        }
    }
}
