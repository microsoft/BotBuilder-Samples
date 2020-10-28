// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.Slack;
using Microsoft.Bot.Builder.Adapters.Slack.Model;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using SlackAPI;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace Microsoft.BotBuilderSamples.Bots
{
    /// <summary>
    /// An EchoBot class that extends from the ActivityHandler.
    /// </summary>
    public class EchoBot : ActivityHandler
    {
        /// <summary>
        /// OnMessageActivityAsync method that returns an async Task.
        /// </summary>
        /// <param name="turnContext">turnContext of ITurnContext{T}, where T is an IActivity.</param>
        /// <param name="cancellationToken">cancellationToken propagates notifications that operations should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        /// <summary>
        /// OnEventActivityAsync method that returns an async Task.
        /// </summary>
        /// <param name="turnContext">turnContext of ITurnContext{T}, where T is an IActivity.</param>
        /// <param name="cancellationToken">cancellationToken propagates notifications that operations should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == "Command")
            {
                if (turnContext.Activity.Value.ToString() == "/block")
                {
                    var interactiveMessage = MessageFactory.Attachment(
                        CreateInteractiveMessage(
                            Directory.GetCurrentDirectory() + @"\Resources\InteractiveMessage.json"));
                    await turnContext.SendActivityAsync(interactiveMessage, cancellationToken);
                }
            }

            if (turnContext.Activity.Value is EventType slackEvent)
            {
                if (slackEvent.Type == "message")
                {
                    if (slackEvent.AdditionalProperties.ContainsKey("subtype") &&
                        slackEvent.AdditionalProperties["subtype"].ToString() == "file_share")
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Echo: I received an attachment"), cancellationToken);
                    }

                    if (slackEvent.AdditionalProperties.ContainsKey("subtype") &&
                        slackEvent.AdditionalProperties["subtype"].ToString() == "link_shared")
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Echo: I received a link share"), cancellationToken);
                    }
                }
            }
        }

        private static Attachment CreateInteractiveMessage(string filePath)
        {
            var interactiveMessageJson = System.IO.File.ReadAllText(filePath);
            var adaptiveCardAttachment = JsonConvert.DeserializeObject<Block[]>(interactiveMessageJson);

            var blockList = adaptiveCardAttachment.ToList();

            var attachment = new Attachment
            {
                Content = blockList,
                ContentType = "application/json",
                Name = "blocks",
            };

            return attachment;
        }
    }
}
