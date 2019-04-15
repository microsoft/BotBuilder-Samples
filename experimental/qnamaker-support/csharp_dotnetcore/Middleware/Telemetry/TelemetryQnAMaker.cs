// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Middleware.Telemetry
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;

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
        public TelemetryQnaMaker(QnAMakerEndpoint endpoint, QnAMakerOptions options = null, bool logUserName = true, bool logOriginalMessage = true, HttpClient httpClient = null)
            : base(endpoint, options, httpClient)
        {
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
            Endpoint = endpoint;
        }

        public bool LogUserName { get; }

        public bool LogOriginalMessage { get; }

        public QnAMakerEndpoint Endpoint { get; }

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

                if (LogOriginalMessage && options.StrictFilters.Length > 0)
                {

                    var metadata = this.GetMetadata(options.StrictFilters);
                    telemetryProperties.Add(QnATelemetryConstants.OriginalMetadataProperty, metadata);
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
                    for (var i = 0; i < queryResults.Length; i++)
                    {
                        var queryResult = queryResults[i];
                        telemetryProperties.Add(QnATelemetryConstants.QuestionProperty + i.ToString(), string.Join(",", queryResult.Questions));
                        telemetryProperties.Add(QnATelemetryConstants.AnswerProperty + i.ToString(), queryResult.Answer);
                        telemetryProperties.Add(QnATelemetryConstants.ScoreProperty + i.ToString(), queryResult.Score.ToString());
                        if (queryResult.Metadata.Length > 0)
                        {
                            var metadata = this.GetMetadata(queryResult.Metadata);
                            telemetryProperties.Add(QnATelemetryConstants.MetadataProperty + i.ToString(), metadata);
                        }
                    }
                }
                else
                {
                    telemetryProperties.Add(QnATelemetryConstants.QuestionProperty, "No Qna Question matched");
                    telemetryProperties.Add(QnATelemetryConstants.AnswerProperty, "No Qna Question matched");
                }

                telemetryProperties.Add(TelemetryConstants.FromIdProperty, context.Activity.From.Id);
                telemetryProperties.Add(TelemetryConstants.FromNameProperty, context.Activity.From.Name);
                telemetryProperties.Add(SupportBot.Service.TelemetryConstants.ChannelId, context.Activity.ChannelId);
                telemetryProperties.Add(SupportBot.Service.TelemetryConstants.ActivityId, context.Activity.Id);
                if (context.Activity.ReplyToId != null)
                {
                    telemetryProperties.Add(SupportBot.Service.TelemetryConstants.ReplyToId, context.Activity.ReplyToId);
                }

                // Track the event
                ((IBotTelemetryClient)telemetryClient).TrackEvent(QnAMsgEvent, telemetryProperties, telemetryMetrics);
            }

            return queryResults;
        }

        private string GetMetadata(Metadata[] metadatas)
        {
            var result = string.Empty;
            foreach (var metadata in metadatas)
            {
                result = result + metadata.Name + ":" + metadata.Value + ";";
            }

            return result;
        }
    }
}
