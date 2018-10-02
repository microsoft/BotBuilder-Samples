using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Bot.Telemetry
{
    public static class TelemetryLogger
    {
        private static TelemetryClient TelemetryClient { get; } = new TelemetryClient();

        public static void Initialize(string activeInstrumentationKey)
        {
            TelemetryConfiguration.Active.InstrumentationKey = activeInstrumentationKey;
            var builder = new ContainerBuilder();
            builder.RegisterType<DialogActivityLogger>().As<IActivityLogger>().InstancePerLifetimeScope();
            builder.Update(Conversation.Container);
        }

        public static void TrackActivity(IActivity activity, IBotData botData = null)
        {
            var eventTelemetry = BuildEventTelemetry(activity);

            if (botData != null)
            {
                eventTelemetry.Properties.Add("debugActivity", JsonConvert.SerializeObject(activity));
            }

            TelemetryClient.TrackEvent(eventTelemetry);
        }

        private static EventTelemetry BuildEventTelemetry(IActivity activity,
            IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            var eventTelemetry = new EventTelemetry();
            if (activity.Timestamp != null)
            {
                eventTelemetry.Properties.Add("timestamp", GetDateTimeAsIso8601(activity.Timestamp.Value));
            }
            eventTelemetry.Properties.Add("type", activity.Type);
            eventTelemetry.Properties.Add("channel", activity.ChannelId);

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    var messageActivity = activity.AsMessageActivity();
                    if (activity.ReplyToId == null)
                    {
                        eventTelemetry.Name = TelemetryEventTypes.MessageReceived;
                        eventTelemetry.Properties.Add("userId", activity.From.Id);
                        eventTelemetry.Properties.Add("userName", activity.From.Name);
                    }
                    else
                    {
                        eventTelemetry.Name = TelemetryEventTypes.MessageSend;
                    }
                    eventTelemetry.Properties.Add("text", messageActivity.Text);
                    eventTelemetry.Properties.Add("conversationId", messageActivity.Conversation.Id);
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    switch (((Activity) activity).Action)
                    {
                        case ContactRelationUpdateActionTypes.Add:
                            eventTelemetry.Name = TelemetryEventTypes.MessageBotAdded;
                            break;
                        case ContactRelationUpdateActionTypes.Remove:
                            eventTelemetry.Name = TelemetryEventTypes.MessageBotRemoved;
                            break;
                    }
                    break;
                case ActivityTypes.ConversationUpdate:
                    eventTelemetry.Name = TelemetryEventTypes.ConversationUpdate;
                    break;
                case ActivityTypes.EndOfConversation:
                    eventTelemetry.Name = TelemetryEventTypes.ConversationEnded;
                    break;
                default:
                    eventTelemetry.Name = TelemetryEventTypes.MessageOthers;
                    break;
            }

            if (properties != null)
            {
                foreach (var property in properties)
                    eventTelemetry.Properties.Add(property);
            }

            if (metrics == null)
            {
                return eventTelemetry;
            }

            foreach (var metric in metrics)
                eventTelemetry.Metrics.Add(metric);

            return eventTelemetry;
        }

        private static string GetDateTimeAsIso8601(DateTime activity)
        {
            var dateTimeString = JsonConvert.SerializeObject(activity.ToUniversalTime());
            return dateTimeString.Substring(1, dateTimeString.Length - 2);
        }
    }
}