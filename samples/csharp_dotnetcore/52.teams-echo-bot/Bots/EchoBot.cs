// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.StateStorage;
using Microsoft.Bot.Builder.Teams.TeamEchoBot;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
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
            await turnContext.SendActivityAsync($"Turn {state.TurnCount}: You sent '{turnContext.Activity.Text}' {suffixMessage}");
        }
    }
}
