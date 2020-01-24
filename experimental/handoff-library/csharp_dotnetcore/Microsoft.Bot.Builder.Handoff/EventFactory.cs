// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder
{
    /// <summary>
    /// Contains utility methods for creating various event types.
    /// </summary>
    public class EventFactory
    {
        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="handoffContext"></param>
        /// <param name="transcript"></param>
        /// <returns></returns>
        public static IEventActivity CreateHandoffInitiation(ITurnContext turnContext, object handoffContext, Transcript transcript = null)
        {
            var handoffEvent = CreateHandoffEvent(HandoffEventNames.InitiateHandoff, handoffContext, turnContext.Activity.Conversation);

            var conversationReference = turnContext.Activity.GetConversationReference();

            handoffEvent.From = turnContext.Activity.From;
            handoffEvent.RelatesTo = turnContext.Activity.GetConversationReference();
            handoffEvent.ReplyToId = turnContext.Activity.Id;
            handoffEvent.ServiceUrl = turnContext.Activity.ServiceUrl;
            handoffEvent.ChannelId = turnContext.Activity.ChannelId;

            if (transcript != null)
            {
                var bufferedActivities = transcript.Activities.Select(a => a.ApplyConversationReference(conversationReference)).ToArray();
                var attchment = new Attachment
                {
                    Content = new Transcript(bufferedActivities),
                    ContentType = "application/json",
                    Name = "Transcript",
                };
                handoffEvent.Attachments.Add(attchment);
            }

            return handoffEvent;
        }

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="conversation"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IEventActivity CreateHandoffResponse(ConversationAccount conversation, string code)
        {
            return CreateHandoffEvent(HandoffEventNames.HandoffResponse, code, conversation);
        }

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="conversation"></param>
        /// <param name="code"></param>
        /// <param name="transcript"></param>
        /// <returns></returns>
        public static IEventActivity CreateHandoffCompleted(ConversationAccount conversation, string code, Transcript transcript)
        {
            var handoffEvent = CreateHandoffEvent(HandoffEventNames.HandoffResponse, code, conversation);

            if (transcript != null)
            {
                var attchment = new Attachment
                {
                    Content = transcript,
                    ContentType = "application/json",
                    Name = "Transcript",
                };
                handoffEvent.Attachments.Add(attchment);
            }

            return handoffEvent;
        }

        private static Activity CreateHandoffEvent(string name, object value, ConversationAccount conversation)
        {
            var handoffEvent = Activity.CreateEventActivity() as Activity;

            handoffEvent.Name = name;
            handoffEvent.Value = value;
            handoffEvent.Id = Guid.NewGuid().ToString();
            handoffEvent.Timestamp = DateTime.UtcNow;
            handoffEvent.Conversation = conversation;
            handoffEvent.Attachments = new List<Attachment>();
            handoffEvent.Entities = new List<Entity>();
            return handoffEvent;
        }
    }
}
