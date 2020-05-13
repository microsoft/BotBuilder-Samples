// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicsOmnichannelBot.AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using OmniChannel.Models;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class DynamicsOmnichannelAgentBot : ActivityHandler
    {
        private Conversation conversation;
        public DynamicsOmnichannelAgentBot()
        {
            // Load conversation json for demo
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();

            var configuration = configBuilder
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile($"DemoConversation/conversation.json", optional: false, reloadOnChange: true)
                            .Build();

            conversation = new Conversation();
            configuration.Bind("Conversation", conversation);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate && turnContext.Activity.MembersAdded != null)
            {
                var replyActivity = MessageFactory.Text($"{conversation.WelcomeMessage}");
                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            } else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (turnContext.Activity.Text.Contains("agent") | conversation.EscalationDictionary.ContainsKey(turnContext.Activity.Text))
                {
                    Dictionary<string, string> endConversationContext = new Dictionary<string, string>();
                    if (conversation.EscalationDictionary.ContainsKey(turnContext.Activity.Text)) { endConversationContext = conversation.EscalationDictionary[turnContext.Activity.Text]; }

                    await turnContext.SendActivityAsync("Transferring  to an agent, who can help you with this. Please remain online…");

                    Dictionary<string, object> handOffContext = new Dictionary<string, object>()
                    {
                        { "BotHandoffContext", "Specialist request" },
                        { "skill", "service" }
                    };

                    var handoffevent = EventFactory.CreateHandoffInitiation(turnContext, new
                    {
                        MessageToAgent = "Issue Summary: billing question",
                        Context = handOffContext
                    }); // Transcript is persisted by Omnichannel

                    await turnContext.SendActivityAsync(handoffevent);
                }
                else if (turnContext.Activity.Text.ToLower().Contains("end"))
                {
                    await turnContext.SendActivityAsync("Thanks for talking with me. Have a good day. Bye.");
                    IEndOfConversationActivity endOfConversationActivity = Activity.CreateEndOfConversationActivity();
                    await turnContext.SendActivityAsync(endOfConversationActivity);
                }
                else
                {
                    await HandleConversation(turnContext, cancellationToken);
                }
            }
        }

        private async Task HandleConversation(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text;
            IMessageActivity reply = Activity.CreateMessageActivity();

            // Handle conversation scenarios
            if (conversation.ConversationDictionary.ContainsKey(text))
            {
                var replyText = conversation.ConversationDictionary[text];
                if (replyText.Contains("Adaptive Card"))
                {
                    replyText = await GetAdaptiveCardsReplyText(reply, replyText, turnContext, cancellationToken);
                }
                if (replyText.Contains("Suggested Action"))
                {
                    replyText = GetSuggestedActionsReplyText(reply, replyText);
                }
                reply.Text = replyText;
            }
            else {
                switch (text.ToLower())
                {
                    case "microsoft store":
                    case "adaptive card":
                        // Display an Adaptive Card for Microsoft Store
                        reply.Attachments.Add(AdaptiveCards.CreateAdaptiveCardAttachment("store.json"));
                        break;
                    case "microsoft article":
                        // Display an Adaptive Card for support article
                        reply.Attachments.Add(AdaptiveCards.CreateAdaptiveCardAttachment("article.json"));
                        break;
                    case "suggested action":
                        reply.SuggestedActions = AdaptiveCards.CreateSuggestedAction(new string[] { "10am", "1pm", "3pm" });
                        break;
                    default:
                        reply.Text = $"I am sorry, I cannot help you with\n'{text}'\n\n If you would like me to transfer you to a customer service agent please type 'Talk to agent'";
                        break;
                }
            }

            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task<string> GetAdaptiveCardsReplyText(IMessageActivity reply, string replyText, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            int start = replyText.IndexOf("(");
            int end = replyText.IndexOf(")")+1;

            var adaptiveCardSubString = replyText.Substring(start, end - start);

            int cardNameStart = adaptiveCardSubString.IndexOf("'")+1;
            int cardNameEnd = adaptiveCardSubString.LastIndexOf("'");
            var adaptiveCard = adaptiveCardSubString.Substring(cardNameStart, cardNameEnd-cardNameStart);

            reply.Attachments.Add(AdaptiveCards.CreateAdaptiveCardAttachment(adaptiveCard));
            reply.Text = replyText.Replace(adaptiveCardSubString, "");

            await turnContext.SendActivityAsync(reply, cancellationToken);

            // Follow up action or reply to sending adaptive card
            reply.Attachments.Clear();
            return conversation.ConversationDictionary[adaptiveCard];
        }

        private string GetSuggestedActionsReplyText(IMessageActivity reply, string replyText)
        {
            int start = replyText.IndexOf("(");
            int end = replyText.IndexOf(")")+1;

            var suggestedActionSubString = replyText.Substring(start, end-start);

            int startActions = suggestedActionSubString.IndexOf("'")+1;
            int endActions = suggestedActionSubString.LastIndexOf("'");

            var suggestedActions = suggestedActionSubString.Substring(startActions, endActions- startActions).Split("/");

            reply.SuggestedActions = AdaptiveCards.CreateSuggestedAction(suggestedActions);
            return replyText.Replace(suggestedActionSubString, "");
        }
    }
}
