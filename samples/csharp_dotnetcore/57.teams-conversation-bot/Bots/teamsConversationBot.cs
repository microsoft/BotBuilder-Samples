// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsConversationBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();

           // await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            switch (turnContext.Activity.Text)
            {
                case "MentionMe":
                    await MentionActivityAsync(turnContext, cancellationToken);
                    break;

                case "UpdateCardAction":
                    await UpdateCardActivityAsync(turnContext, cancellationToken);
                    break;

                case "Delete":
                    await DeleteCardActivityAsync(turnContext, cancellationToken);
                    break;

                case "MessageAllMembers":
                    await MessageAllMembersAsync(turnContext, cancellationToken);
                    break;

                default:
                    var value = new JObject { { "count", 0 } };
                    
                    var card = new HeroCard
                    {
                        Title = "Welcome Card",
                        Text = "Click the buttons below to update this card",
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type= ActionTypes.MessageBack,
                                Title = "Update Card",
                                Text = "UpdateCardAction",
                                Value = value
                            },
                            new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Title = "Message all members",
                                Text = "MessageAllMembers"
                            }
                        }
                    };

                    await turnContext.SendActivityAsync(MessageFactory.Attachment(card.ToAttachment()));
                    break;
            }
        }

        protected override async Task OnTeamsMembersAddedAsync(IList<ChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to the team {member.Id}."), cancellationToken);
                
            }
        }

        private async Task DeleteCardActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.DeleteActivityAsync(turnContext.Activity.ReplyToId, cancellationToken);
        }

        private async Task MessageAllMembersAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var members = await TeamsInfo.GetMembersAsync(turnContext, cancellationToken);
            foreach (var teamMember in members)
            {
                var connector = turnContext.TurnState.Get<IConnectorClient>();

                var parameters = new ConversationParameters
                {
                    Bot = turnContext.Activity.Recipient,
                    Members = new ChannelAccount[] { teamMember },
                    ChannelData = new TeamsChannelData
                    {
                        Tenant = new TenantInfo
                        {
                            Id = turnContext.Activity.Conversation.TenantId,
                        }
                    },
                };

                var converationReference = await connector.Conversations.CreateConversationAsync(parameters);

                var proactiveMessage = MessageFactory.Text($"Hello {teamMember.Name}. I'm a Teams conversation bot.");
                proactiveMessage.From = turnContext.Activity.Recipient;
                proactiveMessage.Conversation = new ConversationAccount
                {
                    Id = converationReference.Id.ToString(),
                };

                await connector.Conversations.SendToConversationAsync(proactiveMessage, cancellationToken);
            }

            await turnContext.SendActivityAsync(MessageFactory.Text("All members have been messaged"), cancellationToken);
        }

        private async Task UpdateCardActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var data = turnContext.Activity.Value as JObject;
            data["count"] = data["count"].Value<int>() + 1;

            data = JObject.FromObject(data);
            
            var card = new HeroCard
            {
                Title = "Welcome Card",
                Text = $"Update count - {data["count"].Value<int>()}",
                Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type= ActionTypes.MessageBack,
                                Title = "Update Card",
                                Text = "UpdateCardAction",
                                Value = data
                            },
                            new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Title = "Message all members",
                                Text = "MessageAllMembers"
                            },
                            new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                Title = "Delete card",
                                Text = "Delete"
                            }
                        }
            };

            var updatedActivity = MessageFactory.Attachment(card.ToAttachment());

            updatedActivity.Id = turnContext.Activity.ReplyToId;
            
            await turnContext.UpdateActivityAsync(updatedActivity, cancellationToken);            
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
