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
        // NOT SUPPORTED ON TEAMS: AnimationCard, AudioCard, VideoCard, OAuthCard

        const string HeroCard = "Hero";
        const string ThumbnailCard = "Thumbnail";
        const string ReceiptCard = "Receipt";
        const string SigninCard = "Signin";
        const string Carousel = "Carousel";
        const string List = "List";

        static string[] CardTypes = new [] { HeroCard, ThumbnailCard, ReceiptCard, SigninCard, Carousel, List };
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);

            var teamsContext = new TeamsContext(turnContext, null);
            string actualText = teamsContext.GetActivityTextWithoutMentions();
            IMessageActivity reply = null;
            switch (actualText)
            {
                case HeroCard:
                    reply = MessageFactory.Attachment(Cards.GetHeroCard().ToAttachment());
                    break;
                case ThumbnailCard:
                    reply = MessageFactory.Attachment(Cards.GetThumbnailCard().ToAttachment());
                    break;
                case ReceiptCard:
                    reply = MessageFactory.Attachment(Cards.GetReceiptCard().ToAttachment());
                    break;
                case SigninCard:
                    reply = MessageFactory.Attachment(Cards.GetSigninCard().ToAttachment());
                    break;
                case Carousel:
                    // NOTE: if cards are NOT the same height in a carousel, Teams will instead display as AttachmentLayoutTypes.List
                    reply = MessageFactory.Carousel(new[] { Cards.GetHeroCard().ToAttachment(), Cards.GetHeroCard().ToAttachment(), Cards.GetHeroCard().ToAttachment() });
                    break;
                case List:
                    // NOTE: MessageFactory.Attachment with multiple attachments will default to AttachmentLayoutTypes.List
                    reply = MessageFactory.Attachment(new[] { Cards.GetHeroCard().ToAttachment(), Cards.GetHeroCard().ToAttachment(), Cards.GetHeroCard().ToAttachment() });
                    break;
                default:
                    reply = MessageFactory.Attachment(GetChoices());
                    break;
            }
            
            await turnContext.SendActivityAsync(reply);
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
