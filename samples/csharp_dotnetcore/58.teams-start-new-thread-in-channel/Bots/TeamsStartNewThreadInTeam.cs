// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsStartNewThreadInTeam : ActivityHandler
    {
        private string _appId;
        private string _appPassword;

        public TeamsStartNewThreadInTeam(IConfiguration configuration)
        {
            _appId = configuration["MicrosoftAppId"];
            _appPassword = configuration["MicrosoftAppPassword"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamsChannelId = turnContext.Activity.TeamsGetChannelId();
            var message = MessageFactory.Text("This will start a new thread in a channel");

            var serviceUrl = turnContext.Activity.ServiceUrl;
            var credentials = new MicrosoftAppCredentials(_appId, _appPassword);

            var conversationParameters = new ConversationParameters
            {
                IsGroup = true,
                ChannelData = new { channel = new { id = teamsChannelId } },
                Activity = (Activity)message,
            };

            ConversationReference conversationReference = null;

            await ((BotFrameworkAdapter)turnContext.Adapter).CreateConversationAsync(
                teamsChannelId,
                serviceUrl,
                credentials,
                conversationParameters,
                (t, ct) =>
                {
                    conversationReference = t.Activity.GetConversationReference();
                    return Task.CompletedTask;
                },
                cancellationToken);


            await ((BotFrameworkAdapter)turnContext.Adapter).ContinueConversationAsync(
                _appId,
                conversationReference,
                async (t, ct) =>
                {
                    await t.SendActivityAsync(MessageFactory.Text("This will be the first response to the new thread"), ct);
                },
                cancellationToken);
        }
    }
}
