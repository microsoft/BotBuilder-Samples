// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Cards.Bots
{
    public class CardsBot : ActivityHandler
    {
        // NOT SUPPORTED ON TEAMS: AnimationCard, AudioCard, VideoCard, OAuthCard

        /*
         * From the UI you can @mention the bot, from any scope, any of the strings listed below to get that card back.
         */
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

            turnContext.Activity.RemoveRecipientMention();
            IMessageActivity reply = null;
            switch (turnContext.Activity.Text)
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
            var card = new HeroCard();
            card.Title = "Task Module Invocation from Hero Card";
            card.Buttons = new List<CardAction>();
            foreach (var cardType in CardTypes)
            {
                var action = new CardAction(ActionTypes.MessageBack, cardType, text: cardType);
                card.Buttons.Add(action);
            }

            return card.ToAttachment();
        }
    }
}
