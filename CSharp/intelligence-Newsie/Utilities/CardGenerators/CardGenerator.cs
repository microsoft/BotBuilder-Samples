using System.Collections.Generic;
using Microsoft.Bot.Connector;

namespace Newsie.Utilities.CardGenerators
{
    internal class CardGenerator
    {
       // Hero Card with no action button
        public static Attachment GetHeroCard(string title, string subtitle, string text)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text
            };

            return heroCard.ToAttachment();
        }

        public static Attachment GetHeroCard(List<CardAction> cardActions)
        {
            return new HeroCard
            {
                Buttons = cardActions
            }.ToAttachment();
        }

        // Hero Card with no action button
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>
                {
                    cardImage 
                }
            };

            return heroCard.ToAttachment();
        }

        // Hero Card with one action button
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        // Hero Card with two action buttons
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction, CardAction cardAction2)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction, cardAction2 },
            };

            return heroCard.ToAttachment();
        }

        // Hero Card with three action buttons
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction, CardAction cardAction2, CardAction cardAction3)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction, cardAction2, cardAction3 },
            };

            return heroCard.ToAttachment();
        }
    }
}