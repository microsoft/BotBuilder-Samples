// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.AppInsights
{
    /// <summary>
    /// Middleware for logging incoming, outgoing, updated or deleted Activity messages into Application Insights.
    /// In addition, registers the <see cref="TelemetryClient"/> client in the context so other Application Insights
    /// components can log telemetry.
    /// If this <see cref="IMiddleware"/> is removed, all the other sample components don't log (but still operate).
    /// </summary>
    public class MyAppInsightsLoggerMiddleware : IMiddleware
    {
        /// <summary>
        /// The name of the App Insights Middleware in the Context TurnState collection.
        /// </summary>
        public static readonly string AppInsightsServiceKey = $"{nameof(MyAppInsightsLoggerMiddleware)}.AppInsightsContext";

        /// <summary>
        /// Application Insights Custom Event name, logged when new message is received from the user.
        /// </summary>
        public static readonly string BotMsgReceiveEvent = "BotMessageReceived";

        /// <summary>
        /// Application Insights Custom Event name, logged when a message is sent out from the bot.
        /// </summary>
        public static readonly string BotMsgSendEvent = "BotMessageSend";

        /// <summary>
        /// Application Insights Custom Event name, logged when a message is updated by the bot (rare case).
        /// </summary>
        public static readonly string BotMsgUpdateEvent = "BotMessageUpdate";

        /// <summary>
        /// Application Insights Custom Event name, logged when a message is deleted by the bot (rare case).
        /// </summary>
        public static readonly string BotMsgDeleteEvent = "BotMessageDelete";

        private TelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyAppInsightsLoggerMiddleware"/> class.
        /// </summary>
        /// <param name="instrumentationKey">The Application Insights instrumentation key.  See Application Insights for more information.</param>
        /// <param name="logUserName"> (Optional) Enable/Disable logging user name within Application Insights.</param>
        /// <param name="logOriginalMessage"> (Optional) Enable/Disable logging original message name within Application Insights.</param>
        /// <param name="config"> (Optional) TelemetryConfiguration to use for Application Insights.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/application-insights/app-insights-api-custom-events-metrics"/>
        public MyAppInsightsLoggerMiddleware(string instrumentationKey, bool logUserName = false, bool logOriginalMessage = false, TelemetryConfiguration config = null)
        {
            if (string.IsNullOrWhiteSpace(instrumentationKey))
            {
                throw new ArgumentNullException(nameof(instrumentationKey));
            }

            var telemetryConfiguration = config ?? new TelemetryConfiguration(instrumentationKey);
            _telemetryClient = new TelemetryClient(telemetryConfiguration);
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
        }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </summary>
        /// <value>
        /// A value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </value>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/application-insights/app-insights-customer-data"/>
        public bool LogUserName { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the original message into the BotMessageReceived event.
        /// </summary>
        /// <value>
        /// Indicates whether to log the original message into the BotMessageReceived event.
        /// </value>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/application-insights/app-insights-customer-data"/>
        public bool LogOriginalMessage { get; }

        /// <summary>
        /// Records incoming and outgoing activities to the Application Insights store.
        /// </summary>
        /// <param name="context">The <see cref="ITurnContext"/> object for this turn.</param>
        /// <param name="nextTurn">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="ITurnContext"/>
        /// <seealso cref="Bot.Schema.IActivity"/>
        public async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
        {
            BotAssert.ContextNotNull(context);

            if (nextTurn == null)
            {
                throw new ArgumentNullException(nameof(nextTurn));
            }

            context.TurnState.Add(MyAppInsightsLoggerMiddleware.AppInsightsServiceKey, _telemetryClient);

            // log incoming activity at beginning of turn
            if (context.Activity != null)
            {
                var activity = context.Activity;

                // Context properties for App Insights
                if (!string.IsNullOrEmpty(activity.Conversation.Id))
                {
                    _telemetryClient.Context.Session.Id = activity.Conversation.Id;
                }

                if (!string.IsNullOrEmpty(activity.From.Id))
                {
                    _telemetryClient.Context.User.Id = activity.From.Id;
                }

                // Log the Application Insights Bot Message Received
                _telemetryClient.TrackEvent(BotMsgReceiveEvent, FillReceiveEventProperties(activity));
            }

            // hook up onSend pipeline
            context.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);

                foreach (var activity in activities)
                {
                    _telemetryClient.TrackEvent(BotMsgSendEvent, FillSendEventProperties(activity));
                }

                return responses;
            });

            // hook up update activity pipeline
            context.OnUpdateActivity(async (ctx, activity, nextUpdate) =>
            {
                // run full pipeline
                var response = await nextUpdate().ConfigureAwait(false);

                _telemetryClient.TrackEvent(BotMsgUpdateEvent, FillUpdateEventProperties(activity));

                return response;
            });

            // hook up delete activity pipeline
            context.OnDeleteActivity(async (ctx, reference, nextDelete) =>
            {
                // run full pipeline
                await nextDelete().ConfigureAwait(false);

                var deleteActivity = new Activity
                {
                    Type = ActivityTypes.MessageDelete,
                    Id = reference.ActivityId,
                }
                .ApplyConversationReference(reference, isIncoming: false)
                .AsMessageDeleteActivity();

                _telemetryClient.TrackEvent(BotMsgDeleteEvent, FillDeleteEventProperties(deleteActivity));
            });

            if (nextTurn != null)
            {
                await nextTurn(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageReceived.
        /// These properties are logged in the custom event when a new message is received from the user.
        /// </summary>
        /// <param name="activity">The Receive activity to harvest properties to be placed into the Application
        /// Insights custom event.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights
        /// <see cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        /// method for the BotMessageReceived Message.</returns>
        private Dictionary<string, string> FillReceiveEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { AppInsightsConstants.ActivityIDProperty, activity.Id },
                    { AppInsightsConstants.ChannelProperty, activity.ChannelId },
                    { AppInsightsConstants.FromIdProperty, activity.From.Id },
                    { AppInsightsConstants.ConversationIdProperty, activity.Conversation.Id },
                    { AppInsightsConstants.ConversationNameProperty, activity.Conversation.Name },
                    { AppInsightsConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
            if (LogUserName && !string.IsNullOrWhiteSpace(activity.From.Name))
            {
                properties.Add(AppInsightsConstants.FromNameProperty, activity.From.Name);
            }

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(AppInsightsConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageSend.
        /// These properties are logged in the custom event when a response message is sent by the Bot to the user.
        /// </summary>
        /// <param name="activity">The Send <see cref="Activity"/> to harvest properties to be placed into the
        /// Application Insights custom event.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights
        /// <see cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        /// method for the BotMessageSend Message.</returns>
        private Dictionary<string, string> FillSendEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { AppInsightsConstants.ActivityIDProperty, activity.Id },
                    { AppInsightsConstants.ChannelProperty, activity.ChannelId },
                    { AppInsightsConstants.RecipientIdProperty, activity.Recipient.Id },
                    { AppInsightsConstants.ConversationIdProperty, activity.Conversation.Id },
                    { AppInsightsConstants.ConversationNameProperty, activity.Conversation.Name },
                    { AppInsightsConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging user name within Application Insights might be an issue so have provided a config setting to disable this feature
            if (LogUserName && !string.IsNullOrWhiteSpace(activity.Recipient.Name))
            {
                properties.Add(AppInsightsConstants.RecipientNameProperty, activity.Recipient.Name);
            }

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(AppInsightsConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageUpdate.
        /// These properties are logged in the custom event when an activity message is updated by the Bot.
        /// For example, if a card is interacted with by the use, and the card needs to be updated to reflect
        /// some interaction.
        /// </summary>
        /// <param name="activity">The Update <see cref="Activity"/> to harvest properties to be placed into the Application Insights custom event..</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights
        /// <see cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        /// method for the BotMessageUpdate Message.</returns>
        private Dictionary<string, string> FillUpdateEventProperties(Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { AppInsightsConstants.ActivityIDProperty, activity.Id },
                    { AppInsightsConstants.ChannelProperty, activity.ChannelId },
                    { AppInsightsConstants.RecipientIdProperty, activity.Recipient.Id },
                    { AppInsightsConstants.ConversationIdProperty, activity.Conversation.Id },
                    { AppInsightsConstants.ConversationNameProperty, activity.Conversation.Name },
                    { AppInsightsConstants.LocaleProperty, activity.Locale },
                };

            // For some customers, logging the utterances within Application Insights might be an so have provided a config setting to disable this feature
            if (LogOriginalMessage && !string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(AppInsightsConstants.TextProperty, activity.Text);
            }

            return properties;
        }

        /// <summary>
        /// Fills the Application Insights Custom Event properties for BotMessageDelete.
        /// These properties are logged in the custom event when an activity message is deleted by the Bot.  This is a relatively rare case.
        /// </summary>
        /// <param name="activity">The Delete activity to harvest properties to be placed into the Application Insights custom event.</param>
        /// <returns>A dictionary that is sent as "Properties" to Application Insights
        /// <see cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        /// method for the BotMessageDelete Message.</returns>
        private Dictionary<string, string> FillDeleteEventProperties(IMessageDeleteActivity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { AppInsightsConstants.ActivityIDProperty, activity.Id },
                    { AppInsightsConstants.ChannelProperty, activity.ChannelId },
                    { AppInsightsConstants.RecipientIdProperty, activity.Recipient.Id },
                    { AppInsightsConstants.ConversationIdProperty, activity.Conversation.Id },
                    { AppInsightsConstants.ConversationNameProperty, activity.Conversation.Name },
                };

            return properties;
        }

        /// <summary>
        /// Elements that are stored within the Application Insights Custom Event.
        /// These are retrieved from the Incoming Activity.
        /// </summary>
        /// <seealso cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        public static class AppInsightsConstants
        {
            public const string ActivityIDProperty = "ActivityId";
            public const string ChannelProperty = "Channel";
            public const string FromIdProperty = "FromId";
            public const string FromNameProperty = "FromName";
            public const string RecipientIdProperty = "RecipientId";
            public const string RecipientNameProperty = "RecipientName";
            public const string ConversationIdProperty = "ConversationId";
            public const string ConversationNameProperty = "ConversationName";
            public const string TextProperty = "Text";
            public const string LocaleProperty = "Locale";
        }
    }
}
