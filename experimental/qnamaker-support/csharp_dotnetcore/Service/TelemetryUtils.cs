// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Service
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;
    using SupportBot.Dialogs.ShowQnAResult;
    using SupportBot.Providers.QnAMaker;

    /// <summary>
    /// Utilities for logging telemetry.
    /// </summary>
    public static class TelemetryUtils
    {
        /// <summary>
        /// Log personality chat response.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">event name.</param>
        /// <param name="activity">activity.</param>
        /// <param name="qnaStatus">QnA status.</param>
        /// <param name="answer">answer.</param>
        public static void LogNeuroconResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity, ShowQnAResultState qnaStatus, string answer)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.QueryProperty, activity.Text },
                    { TelemetryConstants.AnswerProperty, answer },
                    { TelemetryConstants.NeuroconCountProperty, qnaStatus.NeuroconCount.ToString() },
                };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Log Feedback response.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">event name.</param>
        /// <param name="activity">activity.</param>
        /// <param name="qnastatus">qna status.</param>
        public static void LogFeedbackResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity, ShowQnAResultState qnastatus)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.MetadataName, qnastatus.QnaAnswer.Name },
                    { TelemetryConstants.MetadataParent, qnastatus.QnaAnswer.Parent },
                    { TelemetryConstants.AnswerProperty, qnastatus.QnaAnswer.Text },
                    { TelemetryConstants.QueryProperty, qnastatus.QnaAnswer.Requery},
                    { TelemetryConstants.FeedbackProperty, activity.Text },
                 };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// log welcome message.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">ebent name.</param>
        /// <param name="activity">activity.</param>
        public static void LogWelcomeResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.FeedbackProperty, activity.Text },
                 };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Log train api call.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">event name.</param>
        /// <param name="activity">sctivity.</param>
        /// <param name="activeLearningData">active learning data.</param>
        /// <param name="requery">requery.</param>
        public static void LogTrainApiResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity, ActiveLearningDTO activeLearningData, string requery)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.EndpointKey, activeLearningData.endpointKey },
                    { TelemetryConstants.HostName, activeLearningData.hostName },
                    { TelemetryConstants.Kbid, activeLearningData.kbid },
                    { TelemetryConstants.UserQuestion, activeLearningData.userQuestion },
                    { TelemetryConstants.UserSelectedPrompt, requery },
                };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Log active learning response.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">event name.</param>
        /// <param name="activity">activity.</param>
        /// <param name="activeLearningResponse">active learning response.</param>
        public static void LogActiveLearningResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity, QueryResult activeLearningResponse)
        {
            var properties = new Dictionary<string, string>()
                {
                    { "Answer", activeLearningResponse.Answer },
                    { "OriginalQuestion", activity.Text },
                 };

            foreach (var metadata in activeLearningResponse.Metadata)
            {
                properties.Add(metadata.Name, metadata.Value);
            }

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Log Luis resposne.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="eventName">event name.</param>
        /// <param name="activity">activity.</param>
        /// <param name="topintent">top intent.</param>
        public static void LogLuisResponse(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, string eventName, Activity activity, string topintent)
        {
            var properties = new Dictionary<string, string>()
                {
                    { TelemetryConstants.QueryProperty, activity.Text },
                    { TelemetryConstants.LuisIntent, topintent },
                 };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="activity">activity.</param>
        /// <param name="e">exception</param>
        /// <param name="propertyName">property name.</param>
        /// <param name="propertyValue">property value.</param>

        public static void LogException(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, Activity activity, Exception e, string propertyName = "default", string propertyValue = "default")
        {
            var properties = new Dictionary<string, string>()
            {
                { propertyName, propertyValue },
            };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackException(e);
        }

        /// <summary>
        /// Log event when No answer is shown to the user.
        /// </summary>
        /// <param name="telemetryClient">telemetry client.</param>
        /// <param name="activity">activity.</param>

        public static void LogNoAnswerEvent(Microsoft.ApplicationInsights.TelemetryClient telemetryClient, Activity activity)
        {
            var properties = new Dictionary<string, string>()
            {
                { TelemetryConstants.QueryProperty, activity.Text },
            };

            AddBasicProperties(properties, activity);
            telemetryClient.TrackEvent(TelemetryConstants.NoAnswer, properties);
        }

        private static void AddBasicProperties(Dictionary<string, string> properties, Activity activity)
        {
            properties.Add(TelemetryConstants.FromID, activity.From.Id);
            properties.Add(TelemetryConstants.FromName, activity.From.Name);
            properties.Add(TelemetryConstants.ChannelId, activity.ChannelId);
            properties.Add(TelemetryConstants.ActivityId, activity.Id);
            if (activity.ReplyToId != null)
            {
                properties.Add(TelemetryConstants.ReplyToId, activity.ReplyToId);
            }
        }
    }
}
