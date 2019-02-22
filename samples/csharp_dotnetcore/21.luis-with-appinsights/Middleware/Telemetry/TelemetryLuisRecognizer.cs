// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// TelemetryLuisRecognizer invokes the Luis Recognizer and logs some results into Application Insights.
    /// Logs the Top Intent, Sentiment (label/score), (Optionally) Original Text
    /// Along with Conversation and ActivityID.
    /// See <seealso cref="LuisRecognizer"/> for additional information.
    /// </summary>
    public class TelemetryLuisRecognizer : LuisRecognizer
    {
        private LuisApplication _luisApplication;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLuisRecognizer"/> class.
        /// </summary>
        /// <param name="application">The LUIS application to use to recognize text.</param>
        /// <param name="predictionOptions">The LUIS prediction options to use.</param>
        /// <param name="includeApiResults">TRUE to include raw LUIS API response.</param>
        /// <param name="logPersonalInformation">TRUE to include personally indentifiable information.</param>
        public TelemetryLuisRecognizer(LuisApplication application, LuisPredictionOptions predictionOptions = null, bool includeApiResults = false, bool logPersonalInformation = false)
            : base(application, predictionOptions, includeApiResults)
        {
            _luisApplication = application;

            LogPersonalInformation = logPersonalInformation;
        }

        /// <summary>
        /// Gets a value indicating whether determines whether to log the Activity message text that came from the user.
        /// </summary>
        /// <value>If true, will log the Activity Message text into the AppInsight Custom Event for Luis intents.</value>
        public bool LogPersonalInformation { get; }

        public async Task<T> RecognizeAsync<T>(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
            where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(await RecognizeAsync(dialogContext, cancellationToken).ConfigureAwait(false));
            return result;
        }

        public new async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
            where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(await RecognizeAsync(turnContext, cancellationToken).ConfigureAwait(false));
            return result;
        }

        /// <summary>
        /// Return results of the analysis (Suggested actions and intents), passing the dialog id from dialog context to the TelemetryClient.
        /// </summary>
        /// <param name="dialogContext">Dialog context object containing information for the dialog being executed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The LUIS results of the analysis of the current message text in the current turn's context activity.</returns>
        public async Task<RecognizerResult> RecognizeAsync(DialogContext dialogContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dialogContext == null)
            {
                throw new ArgumentNullException(nameof(dialogContext));
            }

            return await RecognizeInternalAsync(dialogContext.Context, dialogContext.ActiveDialog?.Id, cancellationToken);
        }

        /// <summary>
        /// Return results of the analysis (Suggested actions and intents), using the turn context. This is missing a dialog id used for telemetry..
        /// </summary>
        /// <param name="context">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The LUIS results of the analysis of the current message text in the current turn's context activity.</returns>
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RecognizeInternalAsync(context, null, cancellationToken);
        }

        /// <summary>
        /// Analyze the current message text and return results of the analysis (Suggested actions and intents).
        /// </summary>
        /// <param name="context">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The LUIS results of the analysis of the current message text in the current turn's context activity.</returns>
        private async Task<RecognizerResult> RecognizeInternalAsync(ITurnContext context, string dialogId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Call Luis Recognizer
            var recognizerResult = await base.RecognizeAsync(context, cancellationToken);

            // Find the Telemetry Client
            if (context.TurnState.TryGetValue(TelemetryLoggerMiddleware.AppInsightsServiceKey, out var telemetryClient) && recognizerResult != null)
            {
                var topLuisIntent = recognizerResult.GetTopScoringIntent();
                var intentScore = topLuisIntent.score.ToString("N2");

                // Add the intent score and conversation id properties
                var telemetryProperties = new Dictionary<string, string>()
                {
                    { LuisTelemetryConstants.ApplicationIdProperty, _luisApplication.ApplicationId },
                    { LuisTelemetryConstants.IntentProperty, topLuisIntent.intent },
                    { LuisTelemetryConstants.IntentScoreProperty, intentScore },
                    { LuisTelemetryConstants.FromIdProperty, context.Activity.From.Id },
                };

                if (dialogId != null)
                {
                    telemetryProperties.Add(TelemetryConstants.DialogIdProperty, dialogId);
                }

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

                var entities = recognizerResult.Entities?.ToString();
                telemetryProperties.Add(LuisTelemetryConstants.EntitiesProperty, entities);

                // Use the LogPersonalInformation flag to toggle logging PII data, text is a common example
                if (LogPersonalInformation && !string.IsNullOrEmpty(context.Activity.Text))
                {
                    telemetryProperties.Add(LuisTelemetryConstants.QuestionProperty, context.Activity.Text);
                }

                // Track the event
                ((IBotTelemetryClient)telemetryClient).TrackEvent(LuisTelemetryConstants.IntentPrefix, telemetryProperties);
            }

            return recognizerResult;
        }
    }
}
