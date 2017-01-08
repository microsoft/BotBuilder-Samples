using System.Collections.Generic;
using Microsoft.Bot.Connector;

namespace Newsie.Utilities.CardGenerators
{
    internal class CardGenerator
    {
        public static Attachment GetThumbNailCard(string title = default(string), string subtitle = default(string), string text = default(string), CardImage cardImage = null, List<CardAction> cardActions = null, CardAction cardTap = null)
        {
            var thumbnailCard = new ThumbnailCard
            {
                Title = title
            };

            if (!string.IsNullOrEmpty(subtitle))
            {
                thumbnailCard.Subtitle = subtitle;
            }

            if (!string.IsNullOrEmpty(text))
            {
                thumbnailCard.Text = text;
            }

            if (cardImage != null)
            {
                thumbnailCard.Images = new List<CardImage> { cardImage };
            }

            if (cardActions != null)
            {
                thumbnailCard.Buttons = cardActions;
            }

            if (cardTap != null)
            {
                thumbnailCard.Tap = cardTap;
            }

            return thumbnailCard.ToAttachment();
        }

        public static Attachment GetHeroCard(string title = default(string), string subtitle = default(string), string text = default(string), CardImage cardImage = null, List<CardAction> cardActions = null, CardAction cardTap = null)
        {
            var heroCard = new HeroCard
            {
                Title = title
            };

            if (!string.IsNullOrEmpty(subtitle))
            {
                heroCard.Subtitle = subtitle;
            }

            if (!string.IsNullOrEmpty(text))
            {
                heroCard.Text = text;
            }

            if (cardImage != null)
            {
                heroCard.Images = new List<CardImage> { cardImage };
            }

            if (cardActions != null)
            {
                heroCard.Buttons = cardActions;
            }

            if (cardTap != null)
            {
                heroCard.Tap = cardTap;
            }

            return heroCard.ToAttachment();
        }
    }
}