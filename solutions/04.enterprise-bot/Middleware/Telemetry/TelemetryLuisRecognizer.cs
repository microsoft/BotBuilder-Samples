// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json.Linq;

namespace EnterpriseBot
{
    /// <summary>
    /// TelemetryLuisRecognizer invokes the Luis Recognizer and logs some results into Application Insights.
    /// Logs the Top Intent, Sentiment (label/score), (Optionally) Original Text
    /// Along with Conversation and ActivityID.
    /// The Custom Event name this logs is MyLuisConstants.IntentPrefix + "." + 'found intent name'
    /// For example, if intent name was "add_calender":
    ///    LuisIntent.add_calendar
    /// See <seealso cref="LuisRecognizer"/> for additional information.
    /// </summary>
    public class TelemetryLuisRecognizer : LuisRecognizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLuisRecognizer"/> class.
        /// </summary>
        /// <param name="application">The LUIS application to use to recognize text.</param>
        /// <param name="predictionOptions">The LUIS prediction options to use.</param>
        /// <param name="includeApiResults">TRUE to include raw LUIS API response.</param>
        /// <param name="logOriginalMessage">TRUE to include original user message.</param>
        /// <param name="logUserName">TRUE to include user name.</param>
        public TelemetryLuisRecognizer(LuisApplication application, LuisPredictionOptions predictionOptions = null, bool includeApiResults = false, bool logOriginalMessage = false, bool logUserName = false)
            : base(application, predictionOptions, includeApiResults)
        {
            LogOriginalMessage = logOriginalMessage;
            LogUsername = logUserName;
        }

        /// <summary>
        /// Gets a value indicating whether determines whether to log the Activity message text that came from the user.
        /// </summary>
        /// <value>If true, will log the Activity Message text into the AppInsight Custome Event for Luis intents.</value>
        public bool LogOriginalMessage { get; }

        /// <summary>
        /// Gets a value indicating whether determines whether to log the User name.
        /// </summary>
        /// <value>If true, will log the user name into the AppInsight Custom Event for Luis intents.</value>
        public bool LogUsername { get; }

        /// <summary>
        /// Analyze the current message text and return results of the analysis (Suggested actions and intents).
        /// </summary>
        /// <param name="context">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="logOriginalMessage">Determines if the original message is logged into Application Insights.  This is a privacy consideration.</param>
        /// <returns>The LUIS results of the analysis of the current message text in the current turn's context activity.</returns>
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext context, CancellationToken ct, bool logOriginalMessage = false)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Call Luis Recognizer
            var recognizerResult = await base.RecognizeAsync(context, ct);

            var conversationId = context.Activity.Conversation.Id;

            // Find the Telemetry Client
            if (context.TurnState.TryGetValue(TelemetryLoggerMiddleware.AppInsightsServiceKey, out var telemetryClient) && recognizerResult != null)
            {
                var topLuisIntent = recognizerResult.GetTopScoringIntent();
                var intentScore = topLuisIntent.score.ToString("N2");

                // Add the intent score and conversation id properties
                var telemetryProperties = new Dictionary<string, string>()
                {
                    { LuisTelemetryConstants.ActivityIdProperty, context.Activity.Id },
                    { LuisTelemetryConstants.IntentProperty, topLuisIntent.intent },
                    { LuisTelemetryConstants.IntentScoreProperty, intentScore },
                };

                if (recognizerResult.Properties.TryGetValue("sentiment", out var sentiment) && sentiment is JObject)
                {
                    if (((JObject)sentiment).TryGetValue("label", out var label))
                    {
                        telemetryProperties.Add(LuisTelemetryConstants.SentimentLabelProperty, label.Value<string>());
                    }

                    if (((JObject)sentiment).TryGetValue("score", out var score))
                    {
                        telemetryProperties.Add(LuisTelemetryConstants.SentimentScoreProperty, score.Value<string>());
                    }
                }

                if (!string.IsNullOrEmpty(conversationId))
                {
                    telemetryProperties.Add(LuisTelemetryConstants.ConversationIdProperty, conversationId);
                }

                // Add Luis Entitites
                var entities = new List<string>();
                foreach (var entity in recognizerResult.Entities)
                {
                    if (!entity.Key.ToString().Equals("$instance"))
                    {
                        entities.Add($"{entity.Key}: {entity.Value.First}");
                    }
                }

                // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
                if (logOriginalMessage && !string.IsNullOrEmpty(context.Activity.Text))
                {
                    telemetryProperties.Add(LuisTelemetryConstants.QuestionProperty, context.Activity.Text);
                }

                // Track the event
                ((TelemetryClient)telemetryClient).TrackEvent($"{LuisTelemetryConstants.IntentPrefix}.{topLuisIntent.intent}", telemetryProperties);
            }

            return recognizerResult;
        }
    }
}
