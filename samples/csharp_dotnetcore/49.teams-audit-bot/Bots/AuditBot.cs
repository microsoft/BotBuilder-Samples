// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.AuditBot;
using Microsoft.Bot.Builder.Teams.StateStorage;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class AuditBot : TeamsActivityHandler
    {
        private TeamSpecificConversationState _teamSpecificConversationState;
        private IStatePropertyAccessor<TeamOperationHistory> _auditLog;

        public AuditBot(TeamSpecificConversationState teamSpecificConversationState)
        {
            _teamSpecificConversationState = teamSpecificConversationState;
            _auditLog = teamSpecificConversationState.CreateProperty<TeamOperationHistory>("AuditLogAccessor.AuditLog");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            var teamsContext = turnContext.TurnState.Get<ITeamsContext>();

            string actualText = teamsContext.GetActivityTextWithoutMentions();
            if (actualText.Equals("ShowHistory", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show History", StringComparison.OrdinalIgnoreCase))
            {
                var memberHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

                var replyActivity = ((Activity)turnContext.Activity).CreateReply();

                teamsContext.AddMentionToText(replyActivity, turnContext.Activity.From);
                replyActivity.Text = replyActivity.Text + $" Total of {memberHistory.MemberOperations.Count} operations were performed";

                await turnContext.SendActivityAsync(replyActivity);

                // Going in reverse chronological order.
                for (int i = memberHistory.MemberOperations.Count % 10; i >= 0; i--)
                {
                    var elementsToSend = memberHistory.MemberOperations.Skip(10 * i).Take(10).ToList();

                    var stringBuilder = new StringBuilder();

                    if (elementsToSend.Count > 0)
                    {
                        for (int j = elementsToSend.Count - 1; j >= 0; j--)
                        {
                            stringBuilder.Append($"{elementsToSend[j].ObjectId} --> {elementsToSend[j].Operation} -->  {elementsToSend[j].OperationTime} </br>");
                        }

                        var memberListActivity = ((Activity)turnContext.Activity).CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity);
                    }
                }
            }
            else if (actualText.Equals("ShowCurrentMembers", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Current Members", StringComparison.OrdinalIgnoreCase))
            {
                var teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(
                    turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id)).ToList();

                var replyActivity = ((Activity)turnContext.Activity).CreateReply();
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

                        var memberListActivity = ((Activity)turnContext.Activity).CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity);
                    }
                }
            }
            else if (actualText.Equals("ShowChannelList", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("ShowChannels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channel List", StringComparison.OrdinalIgnoreCase))
            {
                var channelList = await teamsContext.Operations.FetchChannelListAsync(turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id);

                var replyActivity = ((Activity)turnContext.Activity).CreateReply();
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

                        var memberListActivity = ((Activity)turnContext.Activity).CreateReply(stringBuilder.ToString());
                        await turnContext.SendActivityAsync(memberListActivity);
                    }
                }
            }
            else
            {
                await turnContext.SendActivityAsync("Invalid command");
            }
        }

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
            await _teamSpecificConversationState.SaveChangesAsync(turnContext);
        }

        protected override async Task OnTeamMembersAddedEventAsync(IList<ChannelAccount> membersAdded, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            foreach (var memberAdded in membersAdded)
            {
                var teamsContext = turnContext.TurnState.Get<ITeamsContext>();
                var teamsChannelAccount = teamsContext.AsTeamsChannelAccount(memberAdded);

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberAdded",
                });
            }
        }

        protected override async Task OnTeamMembersRemovedEventAsync(IList<ChannelAccount> membersRemoved, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            foreach (var memberRemoved in membersRemoved)
            {
                var teamsContext = turnContext.TurnState.Get<ITeamsContext>();
                var teamsChannelAccount = teamsContext.AsTeamsChannelAccount(memberRemoved);

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberRemoved",
                });
            }
        }

        protected override async Task OnChannelCreatedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelData.Channel.Id,
                Operation = "ChannelCreated",
            });
        }

        protected override async Task OnChannelDeletedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelData.Channel.Id,
                Operation = "ChannelDeleted",
            });
        }

        protected override async Task OnChannelRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelData.Channel.Id,
                Operation = "ChannelRenamed",
            });
        }

        protected override async Task OnTeamRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationHistory = await _auditLog.GetAsync(turnContext, () => new TeamOperationHistory());

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelData.Team.Id,
                Operation = "TeamRenamed",
            });
        }
    }
}
