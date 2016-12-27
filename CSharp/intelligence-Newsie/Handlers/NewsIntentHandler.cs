using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newsie.Extensions;
using Newsie.Models.News;
using Newsie.Services;
using Newsie.Utilities;
using Newsie.Utilities.CardGenerators;

namespace Newsie.Handlers
{
    internal sealed class NewsIntentHandler : IIntentHandler
    {
        private const int NewsMaxTitleChar = 200;
        private const int NewsMaxDescriptionChar = 500;
        private const int NewsMaxProviderChar = 100;
        private const int NewsImageWidth = 382;
        private const int NewsImageHeight = 200;

        private const int NewsMaxResults = 5;

        private readonly ICacheManager cache;
        private readonly IUrlShorteningService urlShorteningService;
        private readonly INewsService bingNewsApiHandler;
        private readonly IBotToUser botToUser;

        public NewsIntentHandler(IBotToUser botToUser, ICacheManager cache, IUrlShorteningService urlShorteningService, INewsService bingNewsApiHandler)
        {
            SetField.NotNull(out this.cache, nameof(cache), cache);
            SetField.NotNull(out this.urlShorteningService, nameof(urlShorteningService), urlShorteningService);
            SetField.NotNull(out this.bingNewsApiHandler, nameof(bingNewsApiHandler), bingNewsApiHandler);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public static NewsieNewsResult PrepareNewsieResultHelper(Value news, NewsieNewsResult newsieResult)
        {
            if (news.description.Length > NewsMaxDescriptionChar)
            {
                newsieResult.ShortenedDescription = news.description.Substring(0, NewsMaxDescriptionChar) + "...";
            }
            else
            {
                newsieResult.ShortenedDescription = news.description;
            }

            if (news.name.Length > NewsMaxTitleChar)
            {
                newsieResult.Name = news.name.Substring(0, NewsMaxTitleChar) + "...";
            }
            else
            {
                newsieResult.Name = news.name;
            }

            if (news.provider[0].name.Length > NewsMaxProviderChar)
            {
                newsieResult.ProviderShortenedName = news.description.Substring(0, NewsMaxProviderChar) + "...";
            }
            else
            {
                newsieResult.ProviderShortenedName = news.provider[0].name;
            }

            // Format date 
            var now = DateTime.Now;

            // If the article is published today, use 'x hours ago' format 
            if (news.datePublished.Year == now.Year && news.datePublished.Month == now.Month &&
                news.datePublished.Day == now.Day)
            {
                string timeSuffix;

                // If published less than 1 hour, use minute(s) suffix
                if (now.Hour - news.datePublished.Hour == 0)
                {
                    timeSuffix = (now.Minute - news.datePublished.Minute <= 1) ? "min ago" : "mins ago";
                    newsieResult.DatePublished = (now.Minute - news.datePublished.Minute) + " " + timeSuffix;
                }
                else
                {
                    timeSuffix = (now.Hour - news.datePublished.Hour) <= 1 ? "hour ago" : "hours ago";
                    newsieResult.DatePublished = (now.Hour - news.datePublished.Hour) + " " + timeSuffix;
                }
            }
            else
            {
                newsieResult.DatePublished =
                    new DateTime(news.datePublished.Year, news.datePublished.Month, news.datePublished.Day).ToString("D");
            }

            newsieResult.ImageContentUrl = news.image?.thumbnail?.contentUrl;

            if (!string.IsNullOrEmpty(newsieResult.ImageContentUrl))
            {
                newsieResult.ImageContentUrl += "&w=" + NewsImageWidth + "&h=" + NewsImageHeight;
            }

            return newsieResult;
        }

        public async Task Respond(IMessageActivity activity, LuisResult result)
        {
            EntityRecommendation entityRecommendation;
            Categories category;
            BingNews bingNews;

            if (result.TryFindEntity(NewsieStrings.NewsEntityCategory, out entityRecommendation) &&
                LuisCategoryParser.TryParse(entityRecommendation.Entity, out category))
            {
                // If a category entity was found issue a category search request
                await this.botToUser.PostAsync(string.Format(Strings.NewsCategoryTypeMessage, category.GetDislaplyName().ToLowerInvariant()));
                bingNews = await this.bingNewsApiHandler.FindNewsByCategory(category.ToString());
            }
            else if (result.TryFindEntity(NewsieStrings.NewsEntityTopic, out entityRecommendation))
            {
                // Else if it no category found try to search for a topic and issue a news search request by topic
                await this.botToUser.PostAsync(string.Format(Strings.NewsTopicTypeMessage, Emojis.Wink));
                bingNews = await this.bingNewsApiHandler.FindNewsByQuery(entityRecommendation.Entity);
            }
            else
            {
                // Otherwise use the whole user message as a query for Bing news search
                await this.botToUser.PostAsync(string.Format(Strings.NewsTopicTypeMessage, Emojis.Wink));
                bingNews = await this.bingNewsApiHandler.FindNewsByQuery(result.Query);
            }

            var reply = this.botToUser.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            for (int i = 0; i < NewsMaxResults; i++)
            {
                var newsieResult = await this.PrepareNewsieResult(bingNews.value[i]);
                var attachment = NewsCardGenerator.GetNewsArticleCard(newsieResult);

                reply.Attachments.Add(attachment);

                this.cache.Write(newsieResult.ShortenedUrl, bingNews.value[i]);
            }

            await this.botToUser.PostAsync(reply);
        }

        private async Task<NewsieNewsResult> PrepareNewsieResult(Value news)
        {
            var newsieResult = new NewsieNewsResult
            {
                ShortenedUrl = await this.urlShorteningService.GetShortenedUrl(news.url),
                Url = news.url
            };

            return PrepareNewsieResultHelper(news, newsieResult);
        }
    }
}