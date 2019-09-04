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
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;
        private UserState _userState;
        private IStatePropertyAccessor<string> _joshID;
        private IConfiguration _config;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState, UserState userState, IConfiguration config)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
            _userState = userState;
            _joshID = userState.CreateProperty<string>("joshID");
            _config = config;
        }


        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var id = _joshID.GetAsync(turnContext, () => { return "1"; }).Result;

            if (id == turnContext.Activity?.ReplyToId)
            {
                if (turnContext.Activity.Text == "deleteMe")
                {
                    
                    var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl), _config["MicrosoftAppId"], _config["MicrosoftAppPassword"]);
                    MicrosoftAppCredentials.TrustServiceUrl(turnContext.Activity.ServiceUrl);

                    await connector.Conversations.DeleteActivityAsync(turnContext.Activity.Conversation.Id, id, cancellationToken);
                }
                else
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Id = id;

                    var heroCard = new HeroCard
                    {
                        Title = "BF hero card v2"
                    };

                    reply.Attachments.Add(heroCard.ToAttachment());

                    await turnContext.UpdateActivityAsync(reply, cancellationToken);
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
                Title = "BF hero card",
                Text = "build stuff",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.MessageBack, text:"clickMe", value:"button1"),
                    new CardAction(ActionTypes.MessageBack, text:"deleteMe", value:"button2")
                },
            };
            
            var replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments.Add(heroCard.ToAttachment());
            var result = await turnContext.SendActivityAsync(replyActivity, cancellationToken);

            await _joshID.SetAsync(turnContext, result.Id, cancellationToken);
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}
