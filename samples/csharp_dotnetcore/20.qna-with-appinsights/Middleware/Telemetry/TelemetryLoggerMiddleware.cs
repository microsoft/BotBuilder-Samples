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
using QnABotAppInsights.Middleware.Telemetry;

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

        private IBotTelemetryClient _telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLoggerMiddleware"/> class.
        /// </summary>
        /// <param name="telemetryClient">The IBotTelemetryClient implementation used for registering telemetry events.</param>
        /// <param name="logPersonalInformation"> (Optional) TRUE to include personally indentifiable information.</param>
        /// <param name="config"> (Optional) TelemetryConfiguration to use for Application Insights.</param>
        public TelemetryLoggerMiddleware(IBotTelemetryClient telemetryClient, bool logPersonalInformation = false, TelemetryConfiguration config = null)
        {
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            LogPersonalInformation = logPersonalInformation;
        }

        /// <summary>
        /// Gets a value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </summary>
        /// <value>
        /// A value indicating whether indicates whether to log the user name into the BotMessageReceived event.
        /// </value>
        public bool LogPersonalInformation { get; }

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
                _telemetryClient.TrackEvent(TelemetryLoggerConstants.BotMsgReceiveEvent, this.FillReceiveEventProperties(activity));
            }

            // hook up onSend pipeline
            context.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);

                foreach (var activity in activities)
                {
                    _telemetryClient.TrackEvent(TelemetryLoggerConstants.BotMsgSendEvent, this.FillSendEventProperties(activity));
                }

                return responses;
            });

            // hook up update activity pipeline
            context.OnUpdateActivity(async (ctx, activity, nextUpdate) =>
            {
                // run full pipeline
                var response = await nextUpdate().ConfigureAwait(false);

                _telemetryClient.TrackEvent(TelemetryLoggerConstants.BotMsgUpdateEvent, this.FillUpdateEventProperties(activity));

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

                _telemetryClient.TrackEvent(TelemetryLoggerConstants.BotMsgDeleteEvent, this.FillDeleteEventProperties(deleteActivity));
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
                    { TelemetryConstants.RecipientIdProperty, activity.Recipient.Id },
                    { TelemetryConstants.RecipientNameProperty, activity.Recipient.Name },
                };

            // Use the LogPersonalInformation flag to toggle logging PII data, text and user name are common examples
            if (LogPersonalInformation)
            {
                if (!string.IsNullOrWhiteSpace(activity.From.Name))
                {
                    properties.Add(TelemetryConstants.FromNameProperty, activity.From.Name);
                }

                if (!string.IsNullOrWhiteSpace(activity.Text))
                {
                    properties.Add(TelemetryConstants.TextProperty, activity.Text);
                }

                if (!string.IsNullOrWhiteSpace(activity.Speak))
                {
                    properties.Add(TelemetryConstants.SpeakProperty, activity.Speak);
                }
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
                    { TelemetryConstants.ReplyActivityIDProperty, activity.ReplyToId },
                    { TelemetryConstants.RecipientIdProperty, activity.Recipient.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                    { TelemetryConstants.LocaleProperty, activity.Locale },
                };

            // Use the LogPersonalInformation flag to toggle logging PII data, text and user name are common examples
            if (LogPersonalInformation)
            {
                if (!string.IsNullOrWhiteSpace(activity.Recipient.Name))
                {
                    properties.Add(TelemetryConstants.RecipientNameProperty, activity.Recipient.Name);
                }

                if (!string.IsNullOrWhiteSpace(activity.Text))
                {
                    properties.Add(TelemetryConstants.TextProperty, activity.Text);
                }

                if (!string.IsNullOrWhiteSpace(activity.Speak))
                {
                    properties.Add(TelemetryConstants.SpeakProperty, activity.Speak);
                }
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
                    { TelemetryConstants.ConversationIdProperty, activity.Conversation.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                    { TelemetryConstants.LocaleProperty, activity.Locale },
                };

            // Use the LogPersonalInformation flag to toggle logging PII data, text is a common example
            if (LogPersonalInformation && !string.IsNullOrWhiteSpace(activity.Text))
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
                    { TelemetryConstants.ConversationIdProperty, activity.Conversation.Id },
                    { TelemetryConstants.ConversationNameProperty, activity.Conversation.Name },
                };

            return properties;
        }
    }
}
