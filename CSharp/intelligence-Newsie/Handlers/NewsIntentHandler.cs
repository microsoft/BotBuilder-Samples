using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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
                    timeSuffix = (now.Minute - news.datePublished.Minute <= 1) ? Strings.SingleMinAgo : Strings.PluralMinAgo;
                    newsieResult.DatePublished = (now.Minute - news.datePublished.Minute) + " " + timeSuffix;
                }
                else
                {
                    timeSuffix = (now.Hour - news.datePublished.Hour) <= 1 ? Strings.SingleHourAgo : Strings.PluralHourAgo;
                    newsieResult.DatePublished = (now.Hour - news.datePublished.Hour) + " " + timeSuffix;
                }
            }
            else
            {
                newsieResult.DatePublished = new DateTime(news.datePublished.Year, news.datePublished.Month, news.datePublished.Day).ToString("D");
            }

            if (!string.IsNullOrEmpty(news.image?.thumbnail?.contentUrl))
            {
                newsieResult.ImageContentUrl = news.image.thumbnail.contentUrl;
                newsieResult.ImageContentUrl += "&w=" + news.image.thumbnail.width + "&h=" + news.image.thumbnail.height;
            }

            newsieResult.Url = news.url;

            return newsieResult;
        }

        public async Task Respond(IMessageActivity activity, LuisResult result)
        {
            EntityRecommendation entityRecommendation;
            NewsCategory newsCategory = NewsCategory.None;
            BingNews bingNews;

            if (result.TryFindEntity(NewsieStrings.NewsEntityCategory, out entityRecommendation) &&
                NewsCategoryParser.TryParse(entityRecommendation.Entity, out newsCategory))
            {
                // If a category entity was found issue a category search request
                await this.botToUser.PostAsync(string.Format(Strings.NewsCategoryTypeMessage, newsCategory.GetDislaplyName().ToLowerInvariant()));
                bingNews = await this.bingNewsApiHandler.FindNewsByCategory(newsCategory.ToString());
            }
            else if (result.TryFindEntity(NewsieStrings.NewsEntityTopic, out entityRecommendation))
            {
                // Else if it no category found try to search for a topic and issue a news search request by topic
                await this.botToUser.PostAsync(string.Format(Strings.NewsTopicTypeMessage));
                bingNews = await this.bingNewsApiHandler.FindNewsByQuery(entityRecommendation.Entity);
            }
            else
            {
                // Otherwise use the whole user message as a query for Bing news search
                await this.botToUser.PostAsync(string.Format(Strings.NewsTopicTypeMessage));
                bingNews = await this.bingNewsApiHandler.FindNewsByQuery(result.Query);
            }

            var reply = this.botToUser.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            for (int i = 0; i < NewsMaxResults; i++)
            {
                var newsieResult = await this.PrepareNewsieResult(bingNews.value[i]);
                var attachments = NewsCardGenerator.GetNewsArticleCard(newsieResult, activity.ChannelId);

                foreach (var attachment in attachments)
                {
                    reply.Attachments.Add(attachment);
                }

                this.cache.Write(newsieResult.ShortenedUrl, bingNews.value[i]);
            }

            if (newsCategory != NewsCategory.None)
            {
                reply.Attachments.Add(CardGenerator.GetHeroCard(
                    cardActions: new List<CardAction>
                    {
                        new CardAction(ActionTypes.OpenUrl, Strings.BingForMore, value: $"https://www.bing.com/news?q={newsCategory.GetQueryName()}+news")
                    },
                    cardImage: new CardImage(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Content/binglogo.jpg")));
            }

            await this.botToUser.PostAsync(reply);
        }

        private async Task<NewsieNewsResult> PrepareNewsieResult(Value news)
        {
            var newsieResult = new NewsieNewsResult
            {
                ShortenedUrl = await this.urlShorteningService.GetShortenedUrl(news.url)
            };

            return PrepareNewsieResultHelper(news, newsieResult);
        }
    }
}