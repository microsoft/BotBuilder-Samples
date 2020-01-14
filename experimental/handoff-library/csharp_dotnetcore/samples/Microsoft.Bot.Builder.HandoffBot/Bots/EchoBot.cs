// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Handoff;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Bot.Builder.EchoBot
{
    class ActivityEx
    {
        public static IEventActivity CreateHandoffEventActivity(ITurnContext turnContext, IEnumerable<IActivity> activities, object handoffContext, CancellationToken cancellationToken = default)
        {
            var conversationReference = turnContext.Activity.GetConversationReference();

            var handoffEvent = Activity.CreateEventActivity() as Activity;

            handoffEvent.Name = "InitiateHandoff";
            handoffEvent.Value = handoffContext;
            handoffEvent.Id = Guid.NewGuid().ToString();
            handoffEvent.Timestamp = DateTime.UtcNow;
            handoffEvent.From = turnContext.Activity.From;
            handoffEvent.RelatesTo = turnContext.Activity.GetConversationReference();
            handoffEvent.ReplyToId = turnContext.Activity.Id;
            handoffEvent.ServiceUrl = turnContext.Activity.ServiceUrl;
            handoffEvent.ChannelId = turnContext.Activity.ChannelId;
            handoffEvent.Conversation = turnContext.Activity.Conversation;
            handoffEvent.Attachments = new List<Attachment>();
            handoffEvent.Entities = new List<Entity>();

            if (activities != null)
            {
                var bufferedActivities = activities.Select(a => a.ApplyConversationReference(conversationReference)).ToArray();
                var attchment = new Attachment
                {
                    Content = bufferedActivities,
                    ContentType = "application/json",
                    Name = "Trasnscript",
                };
                handoffEvent.Attachments.Add(attchment);
            }
            return handoffEvent;
        }

        [Obsolete("This overload is deprecated, use another overload")]
        public static IHandoffActivity CreateHandoffActivity()
        {
            return null;
        }
    }

    public class EchoBot : ActivityHandlerWithHandoff
    {
        private static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"d:\test\1.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"sample.png",
                ContentType = "image/png",
                //ContentUrl = $"data:image/png;base64,{imageData}",
                // ContentUrl = $"https://assets.onestore.ms/cdnfiles/external/uhf/long/9a49a7e9d8e881327e81b9eb43dabc01de70a9bb/images/microsoft-white.png"
                ContentUrl = $"https://www.sample-videos.com/img/Sample-png-image-30mb.png"
            };
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("t1"))
            {
                var msg = MessageFactory.Text($"Sample image with remote attachment");
                msg.Attachments = new List<Attachment>();
                msg.Attachments.Add(new Attachment
                    {
                        Name = @"sample.png",
                        ContentType = "image/png",
                        ContentUrl = $"https://www.sample-videos.com/img/Sample-png-image-30mb.png"
                    });
                await turnContext.SendActivityAsync(msg);
            }
            else if (turnContext.Activity.Text.Contains("agent"))
            {
                await turnContext.SendActivityAsync($"You have requested trasfter to an agent, ConversationId={turnContext.Activity.Conversation.Id}");

                var a1 = MessageFactory.Text($"first message");
                var a2 = MessageFactory.Text($"second message");
                var transcript = new Activity[] { a1, a2 };
                var context = new { Skill = "credit cards" };

                // await turnContext.InitiateHandoffAsyncV2(transcript, context, cancellationToken);

                var handoffEvent = ActivityEx.CreateHandoffEventActivity(turnContext, transcript, context, cancellationToken);
                await turnContext.SendActivityAsync(handoffEvent);

                await turnContext.SendActivityAsync($"Agent transfer has been initiated");

            }
            else if (turnContext.Activity.Text.Contains("human"))
            {
                await turnContext.SendActivityAsync("You will be transferred to a human agent. Sit tight.");

                var a1 = MessageFactory.Text($"first message");
                var a2 = MessageFactory.Text($"second message");
                var transcript = new Activity[] { a1, a2 };
                var context = new { Skill = "credit cards", MSCallerId = "CCI" };
                var request = await turnContext.InitiateHandoffAsync(transcript, context, cancellationToken);

                if (await request.IsCompletedAsync())
                {
                    await turnContext.SendActivityAsync("Handoff request has been completed");
                }
                else
                {
                    await turnContext.SendActivityAsync("Handoff request has NOT been completed");
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
            }
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            var evnt = turnContext.Activity;
            await turnContext.SendActivityAsync($"Received event Name='{evnt.Name}', Status='{evnt.Value}'");
            await base.OnEventActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnHandoffActivityAsync(ITurnContext<IHandoffActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationId = turnContext.Activity.Conversation.Id;
            await turnContext.SendActivityAsync($"Received Handoff ack for conversation {conversationId}");
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
