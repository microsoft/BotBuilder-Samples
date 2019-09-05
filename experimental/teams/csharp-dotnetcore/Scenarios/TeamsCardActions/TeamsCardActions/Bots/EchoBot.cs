// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.BotBuilderSamples;

namespace TeamsCardActions.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text;
            if (!string.IsNullOrEmpty(text) && text.Equals("Show Adaptive Card"))
            {
                var card = AdaptiveCardHelper.GetAdaptiveCard();
                await turnContext.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo Text: {turnContext.Activity.Text} Value: {turnContext.Activity.Value}"), cancellationToken);
                await turnContext.SendActivityAsync(GetMenuActivity(), cancellationToken);
            }
        }

        protected override Task OnTeamMembersAddedEventAsync(IList<ChannelAccount> membersAdded, TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                System.Diagnostics.Debug.Print($"MemberId: {member.Id}");
            }
            return base.OnTeamMembersAddedEventAsync(membersAdded, channelData, turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(GetMenuActivity(), cancellationToken);
                }
            }
        }

        private IMessageActivity GetMenuActivity()
        {
            var buttons = new List<CardAction>();
            buttons.Add(new CardAction()
            {
                Title= ActionTypes.OpenUrl,
                Text = ActionTypes.OpenUrl,
                Type = ActionTypes.OpenUrl,
                Value = "https://bing.com"
            });
            buttons.Add(new CardAction()
            {
                Title = ActionTypes.PostBack,
                Text = ActionTypes.PostBack,
                Type = ActionTypes.PostBack,
                Value = ActionTypes.PostBack
            });
            buttons.Add(new CardAction()
            {
                Title = ActionTypes.ImBack,
                Text = ActionTypes.ImBack,
                Type = ActionTypes.ImBack,
                Value = ActionTypes.ImBack
            });
            buttons.Add(new CardAction()
            {
                Title = ActionTypes.MessageBack,
                Text = ActionTypes.MessageBack,
                Type = ActionTypes.MessageBack,
                Value = ActionTypes.MessageBack
            });
            buttons.Add(new CardAction()
            {
                Title = "Show Adaptive Card",
                Text = "Show Adaptive Card",
                Type = ActionTypes.MessageBack,
                Value = ActionTypes.MessageBack
            });
            var heroCard = new HeroCard("Hello and welcome!", "This sample demonstrates card actions on Teams.", buttons: buttons);
            return MessageFactory.Attachment(heroCard.ToAttachment());
        }
    }
}
