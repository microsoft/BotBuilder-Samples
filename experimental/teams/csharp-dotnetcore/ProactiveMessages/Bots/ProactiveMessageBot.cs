// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples.Bots
{
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

    public class ProactiveMessageBot : TeamsActivityHandler
    {
        private string _appId;
        
        /*
         * This bot should be added to a team, but could work in group chat (with updated onMembersAdded implementations). If you 
         * @mention the bot and send it a message it will "proactivly" message you. See the comment below on the continueConversation call 
         * since proactive messaging can work 2 ways.
         */
        public ProactiveMessageBot(IConfiguration configuration)
        {
            _appId = configuration["MicrosoftAppId"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // You can hand roll a connector to manually address a proactive message
            var connectorClient = turnContext.TurnState.Get<IConnectorClient>();
            var connector = new ConnectorClient(connectorClient.Credentials);

            connector.BaseUri = new Uri(turnContext.Activity.ServiceUrl);

            var parameters = new ConversationParameters
            {
                Bot = turnContext.Activity.From,
                Members = new ChannelAccount[] { turnContext.Activity.From },
                ChannelData = new TeamsChannelData
                {
                    Tenant = new TenantInfo
                    {
                        Id = turnContext.Activity.Conversation.TenantId,
                    },
                },
            };

            var converationReference = await connector.Conversations.CreateConversationAsync(parameters);
            var proactiveMessage = MessageFactory.Text($"Hello {turnContext.Activity.From.Name}. You sent me a message. This is a proactive responsive message.");
            proactiveMessage.From = turnContext.Activity.From;
            proactiveMessage.Conversation = new ConversationAccount
            {
                Id = converationReference.Id.ToString(),
            };

            await connector.Conversations.SendToConversationAsync(proactiveMessage, cancellationToken);

            // Or you can use the adapter to send a message if you already have a conversation reference. You can put this code into the controller if
            // you already have a store of conversation references. 
            await turnContext.Adapter.ContinueConversationAsync(_appId, turnContext.Activity.GetConversationReference(), BotOnTurn, cancellationToken);
        }

        private async Task BotOnTurn (ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Proactive response to the thread.");
        }

        protected override async Task OnTeamsMembersAddedAsync(IList<ChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                var replyActivity = MessageFactory.Text($"{member.Id} was added to the team");
                replyActivity.ApplyConversationReference(turnContext.Activity.GetConversationReference());

                var channelId = turnContext.Activity.Conversation.Id.Split(";")[0];
                replyActivity.Conversation.Id = channelId;
                var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
        }

        protected override async Task OnTeamsMembersRemovedAsync(IList<ChannelAccount> membersRemoved, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersRemoved)
            {
                var replyActivity = MessageFactory.Text($"{member.Id} was removed from the team");
                replyActivity.ApplyConversationReference(turnContext.Activity.GetConversationReference());

                var channelId = turnContext.Activity.Conversation.Id.Split(";")[0];
                replyActivity.Conversation.Id = channelId;
                var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
        }
    }
}
