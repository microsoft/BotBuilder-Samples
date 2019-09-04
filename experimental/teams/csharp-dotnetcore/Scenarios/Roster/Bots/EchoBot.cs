// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.StateStorage;
using Microsoft.Bot.Builder.Teams.TeamEchoBot;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        private IStatePropertyAccessor<EchoState> _accessor;
        private BotState _botState;
        private UserState _userState;
        private IStatePropertyAccessor<string> _joshID;

        public EchoBot(TeamSpecificConversationState teamSpecificConversationState, UserState userState)
        {
            _accessor = teamSpecificConversationState.CreateProperty<EchoState>(EchoStateAccessor.CounterStateName);
            _botState = teamSpecificConversationState;
            _userState = userState;
            _joshID = userState.CreateProperty<string>("joshID");
        }


        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            var teamsContext = turnContext.TurnState.Get<ITeamsContext>();

            string actualText = teamsContext.GetActivityTextWithoutMentions();

            if (actualText.Equals("Show Team Members", StringComparison.OrdinalIgnoreCase)) 
            {


                var teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id));

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
            else if (actualText.Equals("ShowChannels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("ShowChannels", StringComparison.OrdinalIgnoreCase) ||
                actualText.Equals("Show Channels", StringComparison.OrdinalIgnoreCase))
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
            else if (actualText.Equals("show group chat members", StringComparison.OrdinalIgnoreCase))
            {
                var teamMembers = (await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id));

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
            else
            {
                await turnContext.SendActivityAsync("Invalid command. Type \"Show channels\" to see a channel list. Type \"Show members\" to see a list of members in a team. " +
                    "Type \"show group chat members\" to see members in a group chat.");
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            
            await base.OnTurnAsync(turnContext, cancellationToken);
            
        }

        private async Task SendFileCard(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard
            {
                Title = "BF hero card",
                Text = "build stuff",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.MessageBack, text:"clickMe", value:"button1")
                },
            };
            
            var replyActivity = turnContext.Activity.CreateReply();
            replyActivity.Attachments.Add(heroCard.ToAttachment());
            var result = await turnContext.SendActivityAsync(replyActivity, cancellationToken);

            await _joshID.SetAsync(turnContext, result.Id, cancellationToken);
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}
