// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Middleware for logging incoming, outgoing, updated or deleted Activity messages into Application Insights.
    /// In addition, registers the telemetry client in the context so other Application Insights components can log
    /// telemetry.
    /// If this Middleware is removed, all the other sample components don't log (but still operate).
    /// </summary>
    public class TelemetryLoggerMiddleware : IMiddleware
    {
        public static readonly string AppInsightsServiceKey = $"{nameof(TelemetryLoggerMiddleware)}.AppInsightsContext";

        // Application Insights Custom Event name, logged when new message is received from the user
        public static readonly string BotMsgReceiveEvent = "BotMessageReceived";

        // Application Insights Custom Event name, logged when a message is sent out from the bot
        public static readonly string BotMsgSendEvent = "BotMessageSend";

        // Application Insights Custom Event name, logged when a message is updated by the bot (rare case)
        public static readonly string BotMsgUpdateEvent = "BotMessageUpdate";

        // Application Insights Custom Event name, logged when a message is deleted by the bot (rare case)
        public static readonly string BotMsgDeleteEvent = "BotMessageDelete";

        private IBotTelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLoggerMiddleware"/> class.
        /// </summary>
        /// <param name="telemetryClient">The IBotTelemetryClient that logs to Application Insights.</param>
        /// <param name="logUserName"> (Optional) Enable/Disable logging user name within Application Insights.</param>
        /// <param name="logOriginalMessage"> (Optional) Enable/Disable logging original message name within Application Insights.</param>
        /// <param name="config"> (Optional) TelemetryConfiguration to use for Application Insights.</param>
        public TelemetryLoggerMiddleware(IBotTelemetryClient telemetryClient, bool logUserName = false, bool logOriginalMessage = false)
        {
            _telemetryClient = telemetryClient;
            LogUserName = logUserName;
            LogOriginalMessage = logOriginalMessage;
        }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </summary>
        /// <value>
        /// A value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </value>
        public bool LogUserName { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the original message into the BotMessageReceived event.
        /// </summary>
        /// <value>
        /// Indicates whether to log the original message into the BotMessageReceived event.
        /// </value>
        public bool LogOriginalMessage { get; }

        /// <summary>
        /// Records incoming and outgoing activities to the Application Insights store.
        /// </summary>
        /// <param name="context">The context object for this turn.</param>
        /// <param name="nextTurn">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <seealso cref="ITurnContext"/>
        /// <seealso cref="Bot.Schema.IActivity"/>
        public async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
        {
            BotAssert.ContextNotNull(context);

            context.TurnState.Add(TelemetryLoggerMiddleware.AppInsightsServiceKey, _telemetryClient);

            // log incoming activity at beginning of turn
            if (context.Activity != null)
            {
                var activity = context.Activity;

                // Log the Application Insights Bot Message Received
                _telemetryClient.TrackEvent(BotMsgReceiveEvent, this.FillReceiveEventProperties(activity));
            }

            // hook up onSend pipeline
            context.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);

                foreach (var activity in activities)
                {
                    _telemetryClient.TrackEvent(BotMsgSendEvent, this.FillSendEventProperties(activity));
                }

                return responses;
            });

            // hook up update activity pipeline
            context.OnUpdateActivity(async (ctx, activity, nextUpdate) =>
            {
                // run full pipeline
                var response = await nextUpdate().ConfigureAwait(false);

                _telemetryClient.TrackEvent(BotMsgUpdateEvent, this.FillUpdateEventProperties(activity));

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

                _telemetryClient.TrackEvent(BotMsgDeleteEvent, this.FillDeleteEventProperties(deleteActivity));
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
    }
}
