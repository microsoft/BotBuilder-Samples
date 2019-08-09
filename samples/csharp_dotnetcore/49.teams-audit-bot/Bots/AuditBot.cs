// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Abstractions.Teams.ConversationUpdate;
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

        protected override async Task OnTeamMembersAddedEventAsync(TeamMembersAddedEvent teamMembersAddedEvent)
        {
            var conversationHistory = await _auditLog.GetAsync(teamMembersAddedEvent.TurnContext, () => new TeamOperationHistory());

            foreach (var memberAdded in teamMembersAddedEvent.MembersAdded)
            {
                var teamsContext = teamMembersAddedEvent.TurnContext.TurnState.Get<ITeamsContext>();
                var teamsChannelAccount = teamsContext.AsTeamsChannelAccount(memberAdded);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<OperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberAdded",
                    OperationTime = DateTimeOffset.Now,
                });
            }

            await _auditLog.SetAsync(teamMembersAddedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(teamMembersAddedEvent.TurnContext);
        }

        protected override async Task OnTeamMembersRemovedEventAsync(TeamMembersRemovedEvent teamMembersRemovedEvent)
        {
            var conversationHistory = await _auditLog.GetAsync(teamMembersRemovedEvent.TurnContext, () => new TeamOperationHistory());

            foreach (var memberRemoved in teamMembersRemovedEvent.MembersRemoved)
            {
                var teamsContext = teamMembersRemovedEvent.TurnContext.TurnState.Get<ITeamsContext>();
                var teamsChannelAccount = teamsContext.AsTeamsChannelAccount(memberRemoved);

                if (conversationHistory.MemberOperations == null)
                {
                    conversationHistory.MemberOperations = new List<OperationDetails>();
                }

                conversationHistory.MemberOperations.Add(new OperationDetails
                {
                    ObjectId = teamsChannelAccount.AadObjectId,
                    Operation = "MemberRemoved",
                    OperationTime = DateTimeOffset.Now,
                });
            }

            await _auditLog.SetAsync(teamMembersRemovedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(teamMembersRemovedEvent.TurnContext);
        }

        protected override async Task OnChannelCreatedEventAsync(ChannelCreatedEvent channelCreatedEvent)
        {
            var conversationHistory = await _auditLog.GetAsync(channelCreatedEvent.TurnContext, () => new TeamOperationHistory());

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelCreatedEvent.Channel.Id,
                Operation = "ChannelCreated",
                OperationTime = DateTimeOffset.Now,
            });

            await _auditLog.SetAsync(channelCreatedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(channelCreatedEvent.TurnContext);
        }

        protected override async Task OnChannelDeletedEventAsync(ChannelDeletedEvent channelDeletedEvent)
        {
            var conversationHistory = await _auditLog.GetAsync(channelDeletedEvent.TurnContext, () => new TeamOperationHistory());

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelDeletedEvent.Channel.Id,
                Operation = "ChannelDeleted",
                OperationTime = DateTimeOffset.Now,
            });

            await _auditLog.SetAsync(channelDeletedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(channelDeletedEvent.TurnContext);
        }

        protected override async Task OnChannelRenamedEventAsync(ChannelRenamedEvent channelRenamedEvent)
        {
            var conversationHistory = await _auditLog.GetAsync(channelRenamedEvent.TurnContext, () => new TeamOperationHistory());

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = channelRenamedEvent.Channel.Id,
                Operation = "ChannelRenamed",
                OperationTime = DateTimeOffset.Now,
            });

            await _auditLog.SetAsync(channelRenamedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(channelRenamedEvent.TurnContext);
        }

        protected override async Task OnTeamRenamedEventAsync(TeamRenamedEvent teamRenamedEvent)
        {
            TeamOperationHistory conversationHistory = await _auditLog.GetAsync(teamRenamedEvent.TurnContext, () => new TeamOperationHistory()).ConfigureAwait(false);

            if (conversationHistory.MemberOperations == null)
            {
                conversationHistory.MemberOperations = new List<OperationDetails>();
            }

            conversationHistory.MemberOperations.Add(new OperationDetails
            {
                ObjectId = teamRenamedEvent.Team.Id,
                Operation = "TeamRenamed",
                OperationTime = DateTimeOffset.Now,
            });

            await _auditLog.SetAsync(teamRenamedEvent.TurnContext, conversationHistory);
            await _teamSpecificConversationState.SaveChangesAsync(teamRenamedEvent.TurnContext);
        }
    }
}
