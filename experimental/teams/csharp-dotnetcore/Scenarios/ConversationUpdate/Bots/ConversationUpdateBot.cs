// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ConversationUpdateBot : TeamsActivityHandler
    {
        protected override async Task OnChannelRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelData.Channel.Name} is the new Channel name");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnChannelCreatedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelData.Channel.Name} is the Channel created");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnChannelDeletedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelData.Channel.Name} is the Channel deleted");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelData.Channel.Name} is the new Team name");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamMembersAddedEventAsync(IList<ChannelAccount> membersAdded, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersAdded.Select(member => member.Id))} joined {channelData.Team.Name}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamMembersRemovedEventAsync(IList<ChannelAccount> membersRemoved, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersRemoved.Select(member => member.Id))} joined {channelData.Team.Name}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }
    }
}
