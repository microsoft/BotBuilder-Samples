// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams.Internal;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.BotBuilderSamples;

namespace Cards.Bots
{
    public class CardsBot : TeamsActivityHandler
    {
        const string O365ConnectorCard = "O365 Connector";
        const string AdaptiveCard = "Adaptive";
        const string HeroCard = "Hero";
        const string ThumbnailCard = "Thumbnail";
        const string ReceiptCard = "Receipt";
        const string SigninCard = "Signin";
        static string[] CardTypes = new [] { O365ConnectorCard, AdaptiveCard, HeroCard, ThumbnailCard, ReceiptCard, SigninCard };
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            var teamsContext = new TeamsContext(turnContext, null);
            string actualText = teamsContext.GetActivityTextWithoutMentions();
            Attachment attachment=null;
            switch (actualText)
            {
                case O365ConnectorCard:
                    attachment = Cards.CreateSampleO365ConnectorCard().ToAttachment();
                    break;
                case AdaptiveCard:
                    attachment = Cards.CreateAdaptiveCardAttachment();
                    break;
                case HeroCard:
                    attachment = Cards.GetHeroCard().ToAttachment();
                    break;
                case ThumbnailCard:
                    attachment = Cards.GetThumbnailCard().ToAttachment();
                    break;
                case ReceiptCard:
                    attachment = Cards.GetReceiptCard().ToAttachment();
                    break;
                case SigninCard:
                    attachment = Cards.GetSigninCard().ToAttachment();
                    break;
            }
            
            await turnContext.SendActivityAsync(MessageFactory.Attachment(attachment ?? GetChoices()));
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }

        protected override async Task<InvokeResponse> OnO365ConnectorCardActionAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"OnO365ConnectorCardActionAsync event value: {turnContext.Activity.Value.ToString()}"));
            return new InvokeResponse { Status = 200 };
        }
        
        private Attachment GetChoices()
        {
            List<Attachment> cards = new List<Attachment>();

            var adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(new AdaptiveTextBlock("Card Choices"));
            
            foreach (var cardType in CardTypes)
            {
                var action = new CardAction(ActionTypes.MessageBack, cardType, text: cardType);
                adaptiveCard.Actions.Add(action.ToAdaptiveCardAction());
            }

            return adaptiveCard.ToAttachment();
        }
    }
}
