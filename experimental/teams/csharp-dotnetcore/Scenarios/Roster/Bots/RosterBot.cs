// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class RosterBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            var teamsContext = turnContext.TurnState.Get<ITeamsContext>();

            string actualText = teamsContext.GetActivityTextWithoutMentions();

            if (actualText.Equals("Show Team Members", StringComparison.OrdinalIgnoreCase)) 
            {
                await ShowTeamMembers(turnContext, teamsContext, cancellationToken);
            }
            else if (actualText.Equals("ShowChannels", StringComparison.OrdinalIgnoreCase) || actualText.Equals("Show Channels", StringComparison.OrdinalIgnoreCase))
            {
                await ShowChannels(turnContext, teamsContext, cancellationToken);
            }
            else if (actualText.Equals("show group chat members", StringComparison.OrdinalIgnoreCase))
            {
                await ShowGroupChatMembers(turnContext, teamsContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync("Invalid command. Type \"Show channels\" to see a channel list. Type \"Show members\" to see a list of members in a team. " +
                    "Type \"show group chat members\" to see members in a group chat.");
            }
        }

        private async Task ShowTeamMembers(ITurnContext<IMessageActivity> turnContext, ITeamsContext teamsContext, CancellationToken cancellationToken)
        {
            var teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id));

            var replyActivity = turnContext.Activity.CreateReply();
            teamsContext.AddMentionToText(replyActivity, turnContext.Activity.From);
            replyActivity.Text = replyActivity.Text + $" Total of {teamMembers.Count} members are currently in team";

            await turnContext.SendActivityAsync(replyActivity);

            for (int i = teamMembers.Count % 10; i >= 0; i--)
            {
                var elementsToSend = teamMembers.Skip(10 * i).Take(10).ToList().ConvertAll<TeamsChannelAccount>((account) => teamsContext.AsTeamsChannelAccount(account));

                var stringBuilder = new StringBuilder();

                if (elementsToSend.Count > 0)
                {
                    for (int j = elementsToSend.Count - 1; j >= 0; j--)
                    {
                        stringBuilder.Append($"{elementsToSend[j].AadObjectId} --> {elementsToSend[j].Name} -->  {elementsToSend[j].UserPrincipalName} </br>");
                    }

                    var memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                    await turnContext.SendActivityAsync(memberListActivity);
                }
            }
        }

        private async Task ShowChannels(ITurnContext<IMessageActivity> turnContext, ITeamsContext teamsContext, CancellationToken cancellationToken)
        {
            var channelList = await teamsContext.Operations.FetchChannelListAsync(turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id);

            var replyActivity = turnContext.Activity.CreateReply();
            teamsContext.AddMentionToText(replyActivity, turnContext.Activity.From);
            replyActivity.Text = replyActivity.Text + $" Total of {channelList.Conversations.Count} channels are currently in team";

            await turnContext.SendActivityAsync(replyActivity);

            for (int i = channelList.Conversations.Count % 10; i >= 0; i--)
            {
                var elementsToSend = channelList.Conversations.Skip(10 * i).Take(10).ToList();

                var stringBuilder = new StringBuilder();

                if (elementsToSend.Count > 0)
                {
                    for (int j = elementsToSend.Count - 1; j >= 0; j--)
                    {
                        stringBuilder.Append($"{elementsToSend[j].Id} --> {elementsToSend[j].Name}</br>");
                    }

                    var memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                    await turnContext.SendActivityAsync(memberListActivity);
                }
            }
        }

        private async Task ShowGroupChatMembers(ITurnContext<IMessageActivity> turnContext, ITeamsContext teamsContext, CancellationToken cancellationToken)
        {
            var teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id));

            var replyActivity = turnContext.Activity.CreateReply();
            teamsContext.AddMentionToText(replyActivity, turnContext.Activity.From);
            replyActivity.Text = replyActivity.Text + $" Total of {teamMembers.Count} members are currently in team";

            await turnContext.SendActivityAsync(replyActivity);

            for (int i = teamMembers.Count % 10; i >= 0; i--)
            {
                var elementsToSend = teamMembers.Skip(10 * i).Take(10).ToList().ConvertAll<TeamsChannelAccount>((account) => teamsContext.AsTeamsChannelAccount(account));

                var stringBuilder = new StringBuilder();

                if (elementsToSend.Count > 0)
                {
                    for (int j = elementsToSend.Count - 1; j >= 0; j--)
                    {
                        stringBuilder.Append($"{elementsToSend[j].AadObjectId} --> {elementsToSend[j].Name} -->  {elementsToSend[j].UserPrincipalName} </br>");
                    }

                    var memberListActivity = turnContext.Activity.CreateReply(stringBuilder.ToString());
                    await turnContext.SendActivityAsync(memberListActivity);
                }
            }
        }
    }
}
