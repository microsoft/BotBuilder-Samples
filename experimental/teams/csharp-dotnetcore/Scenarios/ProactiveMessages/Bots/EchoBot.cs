// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.StateStorage;
using Microsoft.Bot.Builder.Teams.TeamEchoBot;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;
        private IStatePropertyAccessor<string> _joshID;
        private IConfiguration _config;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState, UserState userState, IConfiguration config)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
            _config = config;
        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            if (channelData.EventType != null)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    var userId = turnContext.Activity.MembersAdded[0].Id;
                    var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), _config["MicrosoftAppId"], _config["MicrosoftAppPassword"]);
                    MicrosoftAppCredentials.TrustServiceUrl(turnContext.Activity.ServiceUrl);


                    var parameters = new ConversationParameters

                    {
                        Bot = turnContext.Activity.From,
                        Members = new ChannelAccount[] { new ChannelAccount(userId) },
                        ChannelData = new TeamsChannelData
                        {
                            Tenant = channelData.Tenant

                        }
                    };

                    var conversationResource = await connector.Conversations.CreateConversationAsync(parameters);

                    var message = Activity.CreateMessageActivity();
                    message.From = turnContext.Activity.From;
                    message.Conversation = new ConversationAccount(id: conversationResource.Id.ToString());
                    message.Text = "This is a proactive message from your friendly neighborhood bot. You were added to a team";

                    await connector.Conversations.SendToConversationAsync((Activity)message);

                }
                else if (turnContext.Activity.MembersRemoved != null)
                {
                    var userId = turnContext.Activity.MembersRemoved[0].Id;
                    var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), _config["MicrosoftAppId"], _config["MicrosoftAppPassword"]);
                    MicrosoftAppCredentials.TrustServiceUrl(turnContext.Activity.ServiceUrl);



                    var parameters = new ConversationParameters

                    {
                        Bot = turnContext.Activity.From,
                        Members = new ChannelAccount[] { new ChannelAccount(userId) },
                        ChannelData = new TeamsChannelData
                        {
                            Tenant = channelData.Tenant

                        }
                    };

                    var conversationResource = await connector.Conversations.CreateConversationAsync(parameters);

                    var message = Activity.CreateMessageActivity();
                    message.From = turnContext.Activity.From;
                    message.Conversation = new ConversationAccount(id: conversationResource.Id.ToString());
                    message.Text = "This is a proactive message from your friendly neighborhood bot. You were removed from a team.";

                    await connector.Conversations.SendToConversationAsync((Activity)message);
                }
            }
            else if (turnContext.Activity.MembersAdded != null)
            {
                var userId = turnContext.Activity.MembersAdded[0].Id;
                var channelID = turnContext.Activity.Conversation.Id;
                Activity newActivity = new Activity()
                {
                    Text = $"{userId} was added to the group chat",
                    Type = ActivityTypes.Message,
                    Conversation = new ConversationAccount
                    {
                        Id = channelID
                    },
                };

                var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), _config["MicrosoftAppId"], _config["MicrosoftAppPassword"]);
                MicrosoftAppCredentials.TrustServiceUrl(turnContext.Activity.ServiceUrl);

                await connector.Conversations.SendToConversationAsync(newActivity, cancellationToken);
            }
            else if (turnContext.Activity.MembersRemoved != null)
            {
                var userId = turnContext.Activity.MembersRemoved[0].Id;
                var channelID = turnContext.Activity.Conversation.Id;
                Activity newActivity = new Activity()
                {
                    Text = $"{userId} was removed from the group chat",
                    Type = ActivityTypes.Message,
                    Conversation = new ConversationAccount
                    {
                        Id = channelID
                    },
                };

                var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), _config["MicrosoftAppId"], _config["MicrosoftAppPassword"]);
                MicrosoftAppCredentials.TrustServiceUrl(turnContext.Activity.ServiceUrl);

                await connector.Conversations.SendToConversationAsync(newActivity, cancellationToken);
            }
            else
            {
                await base.OnTurnAsync(turnContext, cancellationToken);
            }
        }

       
        private async Task SendFileCard(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard
            {
                Title = "Hero Card wired for reactions",
                Text = "Add a message reaction to this card",

            };
            
            var replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments.Add(heroCard.ToAttachment());
            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }
    }
}
