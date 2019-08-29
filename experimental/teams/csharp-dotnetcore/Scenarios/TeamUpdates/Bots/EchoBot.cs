// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.StateStorage;
using Microsoft.Bot.Builder.Teams.TeamEchoBot;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;
        private IStatePropertyAccessor<string> _joshID;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState, UserState userState)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();
            

            

            if (channelData.EventType == "channelRenamed")
            {
                var reply = turnContext.Activity.CreateReply();

                var heroCard = new HeroCard
                {
                    Text = $"{channelData.Channel.Name} is the new Channel name"

                };

                reply.Attachments.Add(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else if (channelData.EventType == "teamRenamed")
            {
                var reply = turnContext.Activity.CreateReply();
                var heroCard = new HeroCard
                {
                    Text = $"{channelData.Team.Name} is the new Team name"

                };
                reply.Attachments.Add(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else if (channelData.EventType == "channelCreated")
            {
                var reply = turnContext.Activity.CreateReply();

                var heroCard = new HeroCard
                {
                    Text = $"{channelData.Channel.Name} is the newly created channel name"

                };

                reply.Attachments.Add(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else if (channelData.EventType == "channelDeleted")
            {
                var reply = turnContext.Activity.CreateReply();

                var heroCard = new HeroCard
                {
                    Text = $"{channelData.Channel.Name} is the newly created channel name"

                };

                reply.Attachments.Add(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                await base.OnTurnAsync(turnContext, cancellationToken);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // --> Get Teams Extensions.
            var teamsContext = turnContext.TurnState.Get<ITeamsContext>();

            var state = await _accessor.GetAsync(turnContext, () => new EchoState()).ConfigureAwait(false);

            state.TurnCount++;

            await _accessor.SetAsync(turnContext, state);

            await _botState.SaveChangesAsync(turnContext);

            string suffixMessage = $"from tenant Id {teamsContext.Tenant.Id}";

            // Echo back to the user whatever they typed.
            await SendFileCard(turnContext, cancellationToken);
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
