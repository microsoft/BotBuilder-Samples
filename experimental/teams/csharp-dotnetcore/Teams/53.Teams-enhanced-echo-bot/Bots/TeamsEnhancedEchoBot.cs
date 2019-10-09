// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsEnhancedEchoBot : TeamsActivityHandler
    {
        private ConcurrentDictionary<string, string> _dict;

        public TeamsEnhancedEchoBot(ConcurrentDictionary<string, string> dict)
        {
            _dict = dict;
        }

        protected override async Task OnTeamsChannelRenamedAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelInfo.Name} is the new Channel name");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamsChannelCreatedAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelInfo.Name} is the Channel created");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamsChannelDeletedAsync(ChannelInfo channelInfo, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{channelInfo.Name} is the Channel deleted");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamsTeamRenamedAsync(TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{teamInfo.Name} is the new Team name");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamsMembersAddedAsync(IList<ChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersAdded.Select(member => member.Id))} joined {teamInfo.Name}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnTeamsMembersRemovedAsync(IList<ChannelAccount> membersRemoved, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersRemoved.Select(member => member.Id))} removed from {teamInfo.Name}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersAdded.Select(member => member.Id))} joined {turnContext.Activity.Conversation.ConversationType}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnMembersRemovedAsync(IList<ChannelAccount> membersRemoved, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard(text: $"{string.Join(' ', membersRemoved.Select(member => member.Id))} removed from {turnContext.Activity.Conversation.ConversationType}");
            await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
        }

        protected override async Task OnReactionsAddedAsync(IList<MessageReaction> messageReactions, ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var reaction in messageReactions)
            {
                // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity that was previously sent from this bot.
                var activityText = "";
                if (_dict.TryGetValue(turnContext.Activity.ReplyToId, out activityText))
                {
                    await SendMessageAndLogActivityId(turnContext, $"You added '{reaction.Type}' regarding '{activityText}'", cancellationToken);
                }
                else
                {
                    // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                    await SendMessageAndLogActivityId(turnContext, $"Activity {turnContext.Activity.ReplyToId} not found in the log.", cancellationToken);
                }
            }
        }

        protected override async Task OnReactionsRemovedAsync(IList<MessageReaction> messageReactions, ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var reaction in messageReactions)
            {
                // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity that was previously sent from this bot.
                var activityText = "";
                if (_dict.TryGetValue(turnContext.Activity.ReplyToId, out activityText))
                {
                    await SendMessageAndLogActivityId(turnContext, $"You removed '{reaction.Type}' regarding '{activityText}'", cancellationToken);
                }
                else
                {
                    // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                    await SendMessageAndLogActivityId(turnContext, $"Activity {turnContext.Activity.ReplyToId} not found in the log.", cancellationToken);
                }
            }
        }

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

                case "delete":
                    await DeleteMessages(turnContext, cancellationToken);
                    break;

                case "mention":
                    await MentionActivityAsync(turnContext, cancellationToken);
                    break;

                case "update":
                    await UpdateAllMessages(turnContext, cancellationToken);
                    break;

                default:
                    await turnContext.SendActivityAsync("You can send me \"show members\" from a group chat or team chat to see a list of members in a team. " +
                        "You can send me \"show channels\" from a team to see a channel list for that team. " +
                        "You can send me \"show details\" from a team chat to see information about the team.");
                    break;
            }
        }

        private async Task UpdateAllMessages(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendMessageAndLogActivityId(turnContext, $"{turnContext.Activity.Text}", cancellationToken);
            foreach (var activityId in _dict.Keys)
            {
                var newActivity = MessageFactory.Text(turnContext.Activity.Text);
                newActivity.Id = activityId;
                await turnContext.UpdateActivityAsync(newActivity, cancellationToken);
                _dict[activityId] = turnContext.Activity.Text;
            }
        }

        private async Task MentionActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var mention = new Mention
            {
                Mentioned = turnContext.Activity.From,
                Text = $"<at>{turnContext.Activity.From.Name}</at>",
            };

            // Against Teams having a Mention in the Entities but not including that
            // mention Text in the Activity Text will result in a BadRequest.
            var replyActivity = MessageFactory.Text($"Hello {mention.Text}.");
            replyActivity.Entities = new List<Entity> { mention };

            var responseId = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            _dict.TryAdd(responseId.Id, replyActivity.Id);
        }

        private async Task DeleteMessages(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var activity in _dict.Keys)
            {
                await turnContext.DeleteActivityAsync(activity, cancellationToken);
            }

            _dict.Clear();
        }

        private async Task ShowDetailsAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext, cancellationToken);

            var message = $"The team name is <b>{teamDetails.Name}</b>. The team ID is <b>{teamDetails.Id}</b>. The ADDGroupID is <b>{teamDetails.AadGroupId}</b>.";

            await SendMessageAndLogActivityId(turnContext, message, cancellationToken);
        }

        private async Task ShowMembersAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await ShowMembersAsync(turnContext, await TeamsInfo.GetMembersAsync(turnContext, cancellationToken), cancellationToken);
        }

        private async Task ShowChannelsAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var channels = await TeamsInfo.GetChannelsAsync(turnContext, cancellationToken);

            await SendMessageAndLogActivityId(turnContext, $"Total of {channels.Count} channels are currently in team", cancellationToken);

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

        private async Task SendMessageAndLogActivityId(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            // We need to record the Activity Id from the Activity just sent in order to understand what the reaction is a reaction too. 
            var replyActivity = MessageFactory.Text(text);
            var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            _dict.TryAdd(resourceResponse.Id, replyActivity.Text);
        }

        private async Task SendInBatchesAsync(ITurnContext<IMessageActivity> turnContext, IEnumerable<string> messages, CancellationToken cancellationToken)
        {
            var batch = new List<string>();
            foreach (var msg in messages)
            {
                batch.Add(msg);

                if (batch.Count == 10)
                {
                   var responseId = await turnContext.SendActivityAsync(MessageFactory.Text(string.Join("<br>", batch)), cancellationToken);
                   _dict.TryAdd(responseId.Id, string.Join("<br>", batch));
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
