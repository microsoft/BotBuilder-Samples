// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace LivePersonAgentBot.Bots
{
    public class LivePersonAgentBot : ActivityHandler
    {
        private readonly BotState _conversationState;

        public LivePersonAgentBot(ConversationState conversationState)
        {
            _conversationState = conversationState;
        }

        static Dictionary<string, string> Capitals = new Dictionary<string, string>
        {
            ["France"] = "Paris",
            ["Italy"] = "Rome",
            ["Japan"] = "Tokyo",
            ["Poland"] = "Warsaw",
            ["Germany"] = "Hamburg" // not really
        };

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<LoggingConversationData>(nameof(LoggingConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new LoggingConversationData());

            var userText = turnContext.Activity.Text.ToLowerInvariant();
            if (userText.Contains("agent"))
            {
                await turnContext.SendActivityAsync("Your request will be escalated to a human agent");

                var transcript = new Transcript(conversationData.ConversationLog.Where(a => a.Type == ActivityTypes.Message).ToList());

                var evnt = EventFactory.CreateHandoffInitiation(turnContext,
                    new { Skill = "credit-cards" },
                    transcript);

                await turnContext.SendActivityAsync(evnt);
            }
            else
            {
                string replyText = $"Sorry, I cannot help you.";
                if (userText == "hi")
                {
                    replyText = "Hello!";
                }
                else
                {
                    foreach (var country in Capitals.Keys)
                    {
                        if (userText.Contains(country.ToLower()))
                        {
                            replyText = $"The capital of {country} is {Capitals[country]}";
                        }
                    }
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        protected override async Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if(turnContext.Activity.Name == HandoffEventNames.HandoffStatus)
            {
                var conversationStateAccessors = _conversationState.CreateProperty<LoggingConversationData>(nameof(LoggingConversationData));
                var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new LoggingConversationData());

                Activity replyActivity;
                var state = (turnContext.Activity.Value as JObject)?.Value<string>("state");
                if (state == HandoffStates.Typing)
                {
                    replyActivity = new Activity
                    {
                        Type = ActivityTypes.Typing,
                        Text = "agent is typing",
                    };
                }
                else if (state == HandoffStates.Accepted)
                {
                    replyActivity = MessageFactory.Text("An agent has accepted the conversation and will respond shortly.");
                    await _conversationState.SaveChangesAsync(turnContext);
                }
                else if (state == HandoffStates.Completed)
                {
                    replyActivity = MessageFactory.Text("The agent has closed the conversation.");
                }
                else
                {
                    replyActivity = MessageFactory.Text($"Conversation status changed to '{state}'");
                }

                await turnContext.SendActivityAsync(replyActivity);
            }

            await base.OnEventAsync(turnContext, cancellationToken);
        }
    }
}
