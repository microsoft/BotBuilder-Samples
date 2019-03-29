// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Activity = Microsoft.Bot.Schema.Activity;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Middleware for logging incoming, outgoing, updated or deleted Activity messages.
    /// It logs selected properties to Application Insights, while logs the whole activity messages into Event Hub.
    /// In addition, registers the telemetry client in the context so other components can log
    /// telemetry.
    /// If this Middleware is removed, all the other sample components don't log (but still operate).
    /// </summary>
    public class TelemetryTranscriptLoggerMiddleware : IMiddleware
    {
        public const string TelemetryLogUserName = "telemetry.logUserName";
        public const string TelemetryLogOriginalMessage = "telemetry.logOriginalMessage";

        // Application Insights Custom Event name, logged when new message is received from the user
        public static readonly string BotMsgReceiveEvent = "BotMessageReceived";

        // Application Insights Custom Event name, logged when a message is sent out from the bot
        public static readonly string BotMsgSendEvent = "BotMessageSend";

        // Application Insights Custom Event name, logged when a message is updated by the bot (rare case)
        public static readonly string BotMsgUpdateEvent = "BotMessageUpdate";

        // Application Insights Custom Event name, logged when a message is deleted by the bot (rare case)
        public static readonly string BotMsgDeleteEvent = "BotMessageDelete";

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        private readonly IBotTelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryTranscriptLoggerMiddleware"/> class.
        /// </summary>
        /// <param name="telemetryClient">The IBotTelemetryClient that logs to Application Insights.</param>
        /// <param name="configuration"> The IConfiguration to read a few options, e.g., `LogUserName`.</param>
        public TelemetryTranscriptLoggerMiddleware(IBotTelemetryClient telemetryClient, IConfiguration configuration)
        {
            var logUserName = configuration.GetValue<bool>(TelemetryLogUserName);
            var logOriginalMessage = configuration.GetValue<bool>(TelemetryLogOriginalMessage);

            _telemetryClient = telemetryClient;
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
        }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the user name into the BotMessageReceived event for AppInsights.
        /// </summary>
        /// <value>
        /// A value indicating whether indicates whether to log the user name into the BotMessageReceived event for AppInsights.
        /// </value>
        public bool LogUserName { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the original message into the BotMessageReceived event for AppInsights.
        /// </summary>
        /// <value>
        /// Indicates whether to log the original message into the BotMessageReceived event for AppInsights.
        /// </value>
        public bool LogOriginalMessage { get; }

        /// <summary>
        /// Records incoming and outgoing activities to the Application Insights store.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="nextTurn">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <seealso cref="ITurnContext"/>
        /// <seealso cref="Bot.Schema.IActivity"/>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate nextTurn, CancellationToken cancellationToken)
        {
            BotAssert.ContextNotNull(turnContext);
            var transcript = new Queue<AppInsightsEvent>();

            // log incoming activity at beginning of turn
            LogIncomingActivity(turnContext, transcript);

            // hook up onSend pipeline
            OnSendActivities(turnContext, transcript);

            // hook up update activity pipeline
            OnUpdateActivity(turnContext, transcript);

            // hook up delete activity pipeline
            OnDeleteActivity(turnContext, transcript);

            if (nextTurn != null)
            {
                await nextTurn(cancellationToken).ConfigureAwait(false);
            }

            FlushTranscript(transcript);
        }

        private void LogIncomingActivity(ITurnContext turnContext, Queue<AppInsightsEvent> transcript)
        {
            if (turnContext.Activity != null)
            {
                var activity = turnContext.Activity;
                if (turnContext.Activity.From == null)
                {
                    turnContext.Activity.From = new ChannelAccount();
                }

                if (string.IsNullOrEmpty((string)turnContext.Activity.From.Properties["role"]))
                {
                    turnContext.Activity.From.Properties["role"] = "user";
                }

                // Log the Bot Message Received
                var cloned = ActivityHelper.Clone(activity);
                var evt = GetAppInsightsEvent(cloned, BotMsgReceiveEvent, FillReceiveEventProperties);
                LogActivity(transcript, evt);
            }
        }

        private void OnSendActivities(ITurnContext turnContext, Queue<AppInsightsEvent> transcript)
        {
            turnContext.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);

                foreach (var activity in activities)
                {
                    var cloned = ActivityHelper.Clone(activity);
                    var evt = GetAppInsightsEvent(cloned, BotMsgSendEvent, FillSendEventProperties);
                    LogActivity(transcript, evt);
                }

                return responses;
            });
        }

        private void OnUpdateActivity(ITurnContext turnContext, Queue<AppInsightsEvent> transcript)
        {
            turnContext.OnUpdateActivity(async (ctx, activity, nextUpdate) =>
            {
                // run full pipeline
                var response = await nextUpdate().ConfigureAwait(false);

                // add Message Update activity
                var updateActivity = ActivityHelper.Clone(activity);
                updateActivity.Type = ActivityTypes.MessageUpdate;
                var evt = GetAppInsightsEvent(updateActivity, BotMsgUpdateEvent, FillUpdateEventProperties);
                LogActivity(transcript, evt);

                return response;
            });
        }

        private void OnDeleteActivity(ITurnContext turnContext, Queue<AppInsightsEvent> transcript)
        {
            turnContext.OnDeleteActivity(async (ctx, reference, nextDelete) =>
            {
                // run full pipeline
                await nextDelete().ConfigureAwait(false);

                // add MessageDelete activity
                // log as MessageDelete activity
                var deleteActivity = new Activity
                {
                    Type = ActivityTypes.MessageDelete,
                    Id = reference.ActivityId,
                }
                .ApplyConversationReference(reference, isIncoming: false)
                .AsMessageDeleteActivity();

                LogActivity(transcript, GetAppInsightsEvent((Activity)deleteActivity, BotMsgDeleteEvent, FillDeleteEventProperties));
            });
        }

        private void FlushTranscript(Queue<AppInsightsEvent> transcript)
        {
            // flush transcript at end of turn
            while (transcript.Count > 0)
            {
                var appInsightsEvent = transcript.Dequeue();
                _telemetryClient.TrackEvent(appInsightsEvent.CustomType, appInsightsEvent.Properties);
            }
        }

        private static AppInsightsEvent GetAppInsightsEvent(
            Activity clonedActivity,
            string customType,
            Func<Activity, Dictionary<string, string>> fillEventProperties)
        {
            if (clonedActivity.Timestamp == null)
            {
                clonedActivity.Timestamp = DateTime.UtcNow;
            }

            var properties = fillEventProperties(clonedActivity);
            properties[TelemetryConstants.Transcript] = ActivityHelper.ToJson(clonedActivity);

            return new AppInsightsEvent
            {
                CustomType = customType,
                Properties = properties,
            };
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageReceived.
        /// These properties are logged in the custom event when a new message is received from the user.
        /// </summary>
        /// <param name="activity">Last activity sent from user.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights TrackEvent method for the BotMessageReceived Message.</returns>
        private Dictionary<string, string> FillReceiveEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.FromIdProperty, activity.From.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                    { TelemetryConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
            if (LogUserName && !string.IsNullOrWhiteSpace(activity.From.Name))
            {
                properties.Add(TelemetryConstants.FromNameProperty, activity.From.Name);
            }

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(TelemetryConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageSend.
        /// These properties are logged in the custom event when a response message is sent by the Bot to the user.
        /// </summary>
        /// <param name="activity">Last activity sent from user.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights TrackEvent method for the BotMessageSend Message.</returns>
        private Dictionary<string, string> FillSendEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.ReplyActivityIDProperty, activity.Id },
                    { TelemetryConstants.RecipientIdProperty, activity.Recipient.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                    { TelemetryConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
            if (LogUserName && !string.IsNullOrWhiteSpace(activity.Recipient.Name))
            {
                properties.Add(TelemetryConstants.RecipientNameProperty, activity.Recipient.Name);
            }

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(TelemetryConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageUpdate.
        /// These properties are logged in the custom event when an activity message is updated by the Bot.
        /// For example, if a card is interacted with by the use, and the card needs to be updated to reflect
        /// some interaction.
        /// </summary>
        /// <param name="activity">Last activity sent from user.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights TrackEvent method for the BotMessageUpdate Message.</returns>
        private Dictionary<string, string> FillUpdateEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.RecipientIdProperty, activity.Recipient.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                    { TelemetryConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(TelemetryConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageDelete.
        /// These properties are logged in the custom event when an activity message is deleted by the Bot.  This is a relatively rare case.
        /// </summary>
        /// <param name="activity">Last activity sent from user.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights TrackEvent method for the BotMessageDelete Message.</returns>
        private Dictionary<string, string> FillDeleteEventProperties(IMessageDeleteActivity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.RecipientIdProperty, activity.Recipient.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                };

            return properties;
        }

        private void LogActivity(Queue<AppInsightsEvent> transcript, AppInsightsEvent activity) => transcript.Enqueue(activity);


        private class AppInsightsEvent
        {
            public string CustomType { get; set; }

            public IDictionary<string, string> Properties { get; set; }
        }
    }
}
