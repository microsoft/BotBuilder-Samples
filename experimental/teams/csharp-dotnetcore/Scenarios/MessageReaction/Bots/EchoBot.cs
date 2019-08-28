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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;
        private UserState _userState;
        private IStatePropertyAccessor<string> _joshID;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState, UserState userState)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
            _userState = userState;
            _joshID = userState.CreateProperty<string>("joshID");
        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var id = _joshID.GetAsync(turnContext, () => { return "1"; }).Result;

            if (id == turnContext.Activity?.ReplyToId)
            {

                if (turnContext.Activity.Type == "messageReaction")
                {
                    var reply = turnContext.Activity.CreateReply();

                    if (turnContext.Activity.ReactionsAdded != null)
                    {
                        var heroCard = new HeroCard
                        {
                            Title = $"{turnContext.Activity.ReactionsAdded[0].Type} was added"

                        };

                        reply.Attachments.Add(heroCard.ToAttachment());
                    }

                    if (turnContext.Activity.ReactionsRemoved != null)
                    {
                        var heroCard = new HeroCard
                        {
                            Title = $"{turnContext.Activity.ReactionsRemoved[0].Type} was removed"

                        };
                        reply.Attachments.Add(heroCard.ToAttachment());
                    }

                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
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
            var result = await turnContext.SendActivityAsync(replyActivity, cancellationToken);

            await _joshID.SetAsync(turnContext, result.Id, cancellationToken);
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}
