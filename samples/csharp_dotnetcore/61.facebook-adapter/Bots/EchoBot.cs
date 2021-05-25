// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = MessageFactory.Text("Hello and Welcome!");
            await turnContext.SendActivityAsync(activity, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Attachments != null)
            {
                foreach (var attachment in turnContext.Activity.Attachments)
                {
                    var activity = MessageFactory.Text($" I got {turnContext.Activity.Attachments.Count} attachments");

                    var image = new Attachment(
                       attachment.ContentType,
                       content: attachment.Content);

                    activity.Attachments.Add(image);
                    await turnContext.SendActivityAsync(activity, cancellationToken);
                }
            }
            else
            {
                IActivity activity;

                switch (turnContext.Activity.Text)
                {
                    case "button template":
                        activity = MessageFactory.Attachment(CreateTemplateAttachment(Directory.GetCurrentDirectory() + @"/Resources/ButtonTemplatePayload.json"));
                        break;
                    case "media template":
                        activity = MessageFactory.Attachment(CreateTemplateAttachment(Directory.GetCurrentDirectory() + @"/Resources/MediaTemplatePayload.json"));
                        break;
                    case "generic template":
                        activity = MessageFactory.Attachment(CreateTemplateAttachment(Directory.GetCurrentDirectory() + @"/Resources/GenericTemplatePayload.json"));
                        break;
                    case "Hello button":
                        activity = MessageFactory.Text("Hello Human!");
                        break;
                    case "Goodbye button":
                        activity = MessageFactory.Text("Goodbye Human!");
                        break;
                    case "Chatting":
                        activity = MessageFactory.Text("Hello! How can I help you?");
                        break;
                    default:
                        activity = MessageFactory.Text($"Echo: {turnContext.Activity.Text}");
                        break;
                }

                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Value != null)
            {
                var inputs = (Dictionary<string, string>)turnContext.Activity.Value;
                var name = inputs["Name"];

                var activity = MessageFactory.Text($"How are you doing {name}?");
                await turnContext.SendActivityAsync(activity, cancellationToken);
            }
        }

        private static Attachment CreateTemplateAttachment(string filePath)
        {
            var templateAttachmentJson = File.ReadAllText(filePath);
            var templateAttachment = new Attachment()
            {
                ContentType = "template",
                Content = JsonConvert.DeserializeObject(templateAttachmentJson),
            };
            return templateAttachment;
        }
    }
}
