// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ReplyToChannelBot : ActivityHandler
    {
        private string _botId;

        public ReplyToChannelBot(IConfiguration configuration)
        {
            _botId = configuration["MicrosoftAppId"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamChannelId = turnContext.Activity.TeamsGetChannelId();
            var message = MessageFactory.Text("good morning");

            var (conversationReference, activityId) = await turnContext.TeamsCreateConversationAsync(teamChannelId, message, cancellationToken);

            await ((BotFrameworkAdapter)turnContext.Adapter).ContinueConversationAsync(
                _botId,
                conversationReference,
                async (t, ct) =>
                {
                    await t.SendActivityAsync(MessageFactory.Text("good afternoon"), ct);
                    await t.SendActivityAsync(MessageFactory.Text("good night"), ct);
                },
                cancellationToken);
        }
    }
}
