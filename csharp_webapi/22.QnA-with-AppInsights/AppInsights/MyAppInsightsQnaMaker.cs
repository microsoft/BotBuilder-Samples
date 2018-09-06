// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;

namespace QnABot.AppInsights
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
    public class MyAppInsightsQnAMaker : QnAMaker
    {
        public static readonly string QnAMsgEvent = "QnAMessage";

        /// <summary>
        /// Initializes a new instance of the <see cref="MyAppInsightsQnAMaker"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint of the knowledge base to query.</param>
        /// <param name="options">The options for the QnA Maker knowledge base.</param>
        /// <param name="logUserName">Option to log user name to Application Insights (PII consideration).</param>
        /// <param name="logOriginalMessage">Option to log the original message to Application Insights (PII consideration).</param>
        /// <param name="httpClient">An alternate client with which to talk to QnAMaker.
        /// If null, a default client is used for this instance.</param>
        public MyAppInsightsQnAMaker(QnAMakerEndpoint endpoint, QnAMakerOptions options = null, bool logUserName = false, bool logOriginalMessage = false, HttpClient httpClient = null)
            : base(endpoint, options, httpClient)
        {
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
        }

        public bool LogUserName { get; }

        public bool LogOriginalMessage { get; }

        public new async Task<QueryResult[]> GetAnswersAsync(ITurnContext context)
        {
            // Call QnA Maker
            var queryResults = await base.GetAnswersAsync(context);

            // Find the Application Insights Telemetry Client
            if (queryResults != null && context.TurnState.TryGetValue(MyAppInsightsLoggerMiddleware.AppInsightsServiceKey, out var telemetryClient))
            {
                var telemetryProperties = new Dictionary<string, string>();
                var telemetryMetrics = new Dictionary<string, double>();

                // Make it so we can correlate our reports with Activity or Conversation
                telemetryProperties.Add(MyQnAConstants.ActivityIdProperty, context.Activity.Id);
                var conversationId = context.Activity.Conversation.Id;
                if (!string.IsNullOrEmpty(conversationId))
                {
                    telemetryProperties.Add(MyQnAConstants.ConversationIdProperty, conversationId);
                }

                // For some customers, logging original text name within Application Insights might be an issue.
                var text = context.Activity.Text;
                if (LogOriginalMessage && !string.IsNullOrWhiteSpace(text))
                {
                    telemetryProperties.Add(MyQnAConstants.OriginalQuestionProperty, text);
                }

                // For some customers, logging user name within Application Insights might be an issue.
                var userName = context.Activity.From.Name;
                if (LogUserName && !string.IsNullOrWhiteSpace(userName))
                {
                    telemetryProperties.Add(MyQnAConstants.UsernameProperty, userName);
                }

                // Fill in Qna Results (found or not)
                if (queryResults.Length > 0)
                {
                    var queryResult = queryResults[0];
                    telemetryProperties.Add(MyQnAConstants.QuestionProperty, string.Join(",", queryResult.Questions));
                    telemetryProperties.Add(MyQnAConstants.AnswerProperty, queryResult.Answer);
                    telemetryMetrics.Add(MyQnAConstants.ScoreProperty, (double)queryResult.Score);
                }
                else
                {
                    telemetryProperties.Add(MyQnAConstants.QuestionProperty, "No Qna Question matched");
                    telemetryProperties.Add(MyQnAConstants.AnswerProperty, "No Qna Question matched");
                }

                // Track the event
                ((TelemetryClient)telemetryClient).TrackEvent(QnAMsgEvent, telemetryProperties, telemetryMetrics);
            }

            return queryResults;
        }
    }
}