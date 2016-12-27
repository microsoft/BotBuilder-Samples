using Microsoft.Bot.Connector;
using Newsie.Models.News;

namespace Newsie.Utilities.CardGenerators
{
    internal class NewsCardGenerator
    {
        public static Attachment GetNewsArticleCard(NewsieNewsResult news)
        {
            var attachment = CardGenerator.GetHeroCard(
                news.Name,
                news.ProviderShortenedName.ToUpper() + " (" + news.DatePublished + ")",
                news.ShortenedDescription,
                new CardImage(news.ImageContentUrl),
                new CardAction(ActionTypes.ImBack, "Read Summary", value: "summary " + (string.IsNullOrEmpty(news.ShortenedUrl) ? news.Url : news.ShortenedUrl)),
                new CardAction(ActionTypes.OpenUrl, "View on Web", value: news.Url));

            return attachment;
        }

        public static Attachment GetNewsArticleHeadlineCard(NewsieNewsResult news)
        {
            var attachment = CardGenerator.GetHeroCard(
                news.Name,
                news.ProviderShortenedName + ", " + news.DatePublished,
                string.Empty);

            return attachment;
        }

        public static Attachment GetNewsArticleHeadlineImage(NewsieNewsResult news)
        {
            return new Attachment
            {
                ContentUrl = news.ImageContentUrl,
                ContentType = "image/png",
            };
        }
    }
}