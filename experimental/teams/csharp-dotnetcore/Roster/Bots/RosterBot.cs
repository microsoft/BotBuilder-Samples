// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * You need to install this bot in a team. You can @mention the bot "show members", "show channels", or "show details" to get the
     * members of the team, the channels of the team, or metadata about the team respectively.
     */
    public class RosterBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            turnContext.Activity.RemoveRecipientMention();

            switch (turnContext.Activity.Text)
            {
                case "show members":
                    await ShowMembersAsync(turnContext, cancellationToken);
                    break;

                case "show channels":
                    await ShowChannelsAsync(turnContext, cancellationToken);
                    break;

                case "show details":
                    await ShowDetailsAsync(turnContext, cancellationToken);
                    break;

                default:
                    await turnContext.SendActivityAsync("Invalid command. Type \"Show channels\" to see a channel list. Type \"Show members\" to see a list of members in a team. " +
                        "Type \"show group chat members\" to see members in a group chat.");
                    break;
            }
        }

        private async Task ShowDetailsAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext, cancellationToken);

            var replyActivity = MessageFactory.Text($"The team name is {teamDetails.Name}. The team ID is {teamDetails.Id}. The ADDGroupID is {teamDetails.AadGroupId}.");

            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }

        private async Task ShowMembersAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await ShowMembersAsync(turnContext, await TeamsInfo.GetMembersAsync(turnContext, cancellationToken), cancellationToken);
        }

        private async Task ShowChannelsAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var channels = await TeamsInfo.GetChannelsAsync(turnContext, cancellationToken);

            var replyActivity = MessageFactory.Text($"Total of {channels.Count} channels are currently in team");

            await turnContext.SendActivityAsync(replyActivity);

            var messages = channels.Select(channel => $"{channel.Id} --> {channel.Name}");

            await SendInBatchesAsync(turnContext, messages, cancellationToken);
        }

        private async Task ShowMembersAsync(ITurnContext<IMessageActivity> turnContext, IEnumerable<TeamsChannelAccount> teamsChannelAccounts, CancellationToken cancellationToken)
        {
            var replyActivity = MessageFactory.Text($"Total of {teamsChannelAccounts.Count()} members are currently in team");
            await turnContext.SendActivityAsync(replyActivity);

            var messages = teamsChannelAccounts
                .Select(teamsChannelAccount => $"{teamsChannelAccount.AadObjectId} --> {teamsChannelAccount.Name} -->  {teamsChannelAccount.UserPrincipalName}");

            await SendInBatchesAsync(turnContext, messages, cancellationToken);
        }

        private static async Task SendInBatchesAsync(ITurnContext<IMessageActivity> turnContext, IEnumerable<string> messages, CancellationToken cancellationToken)
        {
            var batch = new List<string>();
            foreach (var msg in messages)
            {
                batch.Add(msg);

                if (batch.Count == 10)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Join("<br>", batch)), cancellationToken);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Join("<br>", batch)), cancellationToken);
            }
        }
    }
}
