using System.Collections.Generic;
using Microsoft.Bot.Connector;
using Newsie.Models.News;

namespace Newsie.Utilities.CardGenerators
{
    internal class NewsCardGenerator
    {
        public static IList<Attachment> GetNewsArticleCard(NewsieNewsResult news, string channelId)
        {
            var attachments = new List<Attachment>();

            var name = news.Name;
            var publisher = news.ProviderShortenedName.ToUpper() + " (" + news.DatePublished + ")";
            var image = new CardImage(news.ImageContentUrl);

            var tap = new CardAction(ActionTypes.OpenUrl, "View on Web", value: news.Url);
            var actions = new List<CardAction>
            {
                new CardAction(ActionTypes.ImBack, "Read Summary", value: "summary " + (string.IsNullOrEmpty(news.ShortenedUrl) ? news.Url : news.ShortenedUrl))
            };

            switch (channelId)
            {
                case "slack":
                    attachments.Add(CardGenerator.GetThumbNailCard(publisher, name, news.ShortenedDescription, image, actions));
                    break;
                default:
                    attachments.Add(CardGenerator.GetThumbNailCard(name, publisher, news.ShortenedDescription, image, actions, tap));
                    break;
            }

            return attachments;
        }

        public static Attachment GetNewsArticleHeadlineImageCard(NewsieNewsResult news, string channelId)
        {
            Attachment attachment;
            var name = news.Name;
            var publisher = news.ProviderShortenedName.ToUpper() + " (" + news.DatePublished + ")";
            var image = new CardImage(news.ImageContentUrl);
            var action = new CardAction(ActionTypes.OpenUrl, Strings.FullStoryString, value: news.Url);

            switch (channelId)
            {
                case "slack":
                    attachment = CardGenerator.GetHeroCard(publisher, name, cardImage: image, cardActions: new List<CardAction> { action });
                    break;
                default:
                    attachment = CardGenerator.GetHeroCard(name, publisher, cardImage: image, cardActions: new List<CardAction> { action });
                    break;
            }

            return attachment;
        }
    }
}