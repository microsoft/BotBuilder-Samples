// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsStartNewThreadInTeam : ActivityHandler
    {
        private readonly string _appId;

        public TeamsStartNewThreadInTeam(IConfiguration configuration)
        {
            _appId = configuration["MicrosoftAppId"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamsChannelId = turnContext.Activity.TeamsGetChannelId();
            var activity = MessageFactory.Text("This will start a new thread in a channel");

            var details = await TeamsInfo.SendMessageToTeamsChannelAsync(turnContext, activity, teamsChannelId, _appId, cancellationToken);
            await ((CloudAdapter)turnContext.Adapter).ContinueConversationAsync(
                botAppId: _appId,
                reference: details.Item1,
                callback: async (t, ct) =>
                {
                    await t.SendActivityAsync(MessageFactory.Text("This will be the first response to the new thread"), ct);
                },
                cancellationToken: cancellationToken);
        }
    }
}
