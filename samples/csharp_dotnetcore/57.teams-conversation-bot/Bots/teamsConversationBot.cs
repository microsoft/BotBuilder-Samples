// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsConversationBot : TeamsActivityHandler
    {
        private string _appId;

        public TeamsConversationBot(IConfiguration config)
        {
            _appId = config["MicorosftAppId"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();

            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            switch (turnContext.Activity.Text)
            {
                case "MentionMe":
                    await MentionActivityAsync(turnContext, cancellationToken);
                    break;

                case "ShowCard":
                    await ShowCardActivityAsync(turnContext, cancellationToken);
                    break;

                default:
                    var members = await TeamsInfo.GetMembersAsync(turnContext, cancellationToken);
                    foreach (var member in members)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"AAD: {member.AadObjectId}, ID: {member.Id}"));
                    }
                    var josh = new ChannelAccount
                    {
                        Id = "29:15S79eilr9W40STMFCebm7Nj8wfivnW-2EsYailbRVcSz59K_792I9SYXcpzQYdgwR6OT1u6FZ3qwKuIQsToxqA",
                        AadObjectId = "ae5c5e52-97a9-4688-8472-2c524f29878a",
                        Name = "Josh Ratliff"
                    };

                    var jj = new ChannelAccount
                    {
                        Id = "29:1QA1BHQPG97UoBy0rn3pfG4x4YEM3SLgCNb4ANrJFNYhABzv7OB35lFuL98lQLRDQy-nmlPi8UHbEAB9bgJlPyA",
                        AadObjectId = "8a2e0ffb-dd66-4723-8dce-4cfa6043b459",
                        Name = "JJ"
                    };

                    var eric = new ChannelAccount
                    {
                        Id = "29:1uV7uRAs-ystwLkXp7SjSulgoxjEd4yGZwKeCeaH9JzddtU_o1SGb0MrG4g0wJECQx4cW7NLdcb-Vwe4naKCDVA",
                        AadObjectId = "c87bdf38-be54-466a-bd67-4029da348cc4",
                        Name = "Eric"
                    };

                    var connector = turnContext.TurnState.Get<IConnectorClient>();

                    var parameters = new ConversationParameters
                    {
                        Bot = turnContext.Activity.Recipient,
                        Members = new ChannelAccount[] { josh, jj, eric },
                        ChannelData = new TeamsChannelData
                        {
                            Tenant = new TenantInfo
                            {
                                Id = turnContext.Activity.Conversation.TenantId,
                            }
                        },
                    };

                    var converationReference = await connector.Conversations.CreateConversationAsync(parameters);


                    var proactiveMessage = MessageFactory.Text($"Hello Josh. You sent me a message. This is a proactive responsive message.");
                    proactiveMessage.From = turnContext.Activity.Recipient;
                    proactiveMessage.Conversation = new ConversationAccount
                    {
                        Id = converationReference.Id.ToString(),
                    };

                    await connector.Conversations.SendToConversationAsync(proactiveMessage, cancellationToken);
                    /*var welcomeCard = new HeroCard();
                    var updateCardAction = new CardAction(ActionTypes.)
                    await turnContext.SendActivityAsync("These are the events that I will respond to: " +
                        "Teams channel add, teams channel remove, teams channel rename, team member added, team member removed, message reaction added" +
                        "message reaction removed");*/
                    break;
            }
        }

        protected override async Task OnTeamsMembersAddedAsync(IList<ChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if(member.Name == "TeamsConversationBot")
                {
                    var members = await TeamsInfo.GetMembersAsync(turnContext, cancellationToken);
                    foreach (var teamMember in members)
                    {
                        var message = MessageFactory.Text($"Hello {teamMember.Name}. I'm a Teams conversational bot.");
                        var (conversationReference, activityId)  = await turnContext.TeamsCreateConversationAsync(turnContext.Activity.ChannelId, message, cancellationToken);
                        await turnContext.Adapter.ContinueConversationAsync(_appId, conversationReference, SendMessage, cancellationToken);
                    }
                }
                else
                {
                    var connector = turnContext.TurnState.Get<IConnectorClient>();

                    var parameters = new ConversationParameters
                    {
                        Bot = member,
                        Members = new ChannelAccount[] { member },
                        ChannelData = new TeamsChannelData
                        {
                            Tenant = new TenantInfo
                            {
                                Id = turnContext.Activity.Conversation.TenantId,
                            }
                        },
                    };

                    var converation = await connector.Conversations.CreateConversationAsync(parameters);


                    var proactiveMessage = MessageFactory.Text($"Hello {member.Id}. You sent me a message. This is a proactive responsive message.");
                    
                    proactiveMessage.Conversation = new ConversationAccount
                    {
                        Id = converation.Id.ToString(),
                    };

                    await connector.Conversations.SendToConversationAsync(proactiveMessage, cancellationToken);
                    //var (conversationReference, activityId) = await turnContext.TeamsCreateConversationAsync(turnContext.Activity.Conversation.Id, message, cancellationToken);


                    // message.ApplyConversationReference(conversationReference);
                    //await turnContext.SendActivityAsync(message, cancellationToken);

                    //await turnContext.Adapter.ContinueConversationAsync(_appId, conversationReference, SendMessage, cancellationToken);

                    //await turnContext.SendActivityAsync(message, cancellationToken);
                }
            }
        }

        private async Task SendMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("I'm a bot.");
        }

        private async Task ShowCardActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // read number from message
            // bump the number by 1
            // send new message with bumped number
            await turnContext.SendActivityAsync(MessageFactory.Text("I will delete all messages that I have sent"), cancellationToken);
        }

        private async Task MentionActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var mention = new Mention
            {
                Mentioned = turnContext.Activity.From,
                Text = $"<at>{turnContext.Activity.From.Name}</at>",
            };

            // Against Teams having a Mention in the Entities but not including that
            // mention Text in the Activity Text will result in a BadRequest.
            var replyActivity = MessageFactory.Text($"Hello {mention.Text}.");
            replyActivity.Entities = new List<Entity> { mention };

            var responseId = await turnContext.SendActivityAsync(replyActivity, cancellationToken);

        }
    }
}
