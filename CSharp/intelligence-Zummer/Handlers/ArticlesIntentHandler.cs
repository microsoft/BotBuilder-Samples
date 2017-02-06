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
using Zummer.Models.Search;
using Zummer.Services;
using Zummer.Utilities.CardGenerators;

namespace Zummer.Handlers
{
    internal sealed class ArticlesIntentHandler : IIntentHandler
    {
        private const int ArticlesMaxResults = 5;

        private readonly ISearchService bingSearchApiHandler;
        private readonly IBotToUser botToUser;

        public ArticlesIntentHandler(IBotToUser botToUser, ISearchService bingSearchApiHandler)
        {
            SetField.NotNull(out this.bingSearchApiHandler, nameof(bingSearchApiHandler), bingSearchApiHandler);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IMessageActivity activity, LuisResult result)
        {
            EntityRecommendation entityRecommendation;
            BingSearch bingSearch;
            string query;

            if (result.TryFindEntity(ZummerStrings.ArticlesEntityTopic, out entityRecommendation))
            {
                // Try to find an entity from the user's message via Luis.ai to treat it as Bing Search query
                await this.botToUser.PostAsync(string.Format(Strings.SearchTopicTypeMessage));
                bingSearch = await this.bingSearchApiHandler.FindArticles(query = entityRecommendation.Entity);
            }
            else
            {
                // Otherwise use the whole user's message as a query for Bing Search
                await this.botToUser.PostAsync(string.Format(Strings.SearchTopicTypeMessage));
                bingSearch = await this.bingSearchApiHandler.FindArticles(query = result.Query);
            }

            var reply = this.botToUser.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();

            for (int i = 0; i < ArticlesMaxResults; i++)
            {
                var zummerResult = this.PrepareZummerResult(bingSearch.webPages.value[i]);
                reply.Attachments.Add(ArticleCardGenerator.GetArticleCard(zummerResult, activity.ChannelId));
            }

            reply.Attachments.Add(CardGenerator.GetHeroCard(
                cardActions: new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, Strings.BingForMore, value: $"https://www.bing.com/search/?q={query} site:wikipedia.org")
                },
                cardImage: new CardImage(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Content/binglogo.jpg")));

            await this.botToUser.PostAsync(reply);
        }

        private ZummerSearchResult PrepareZummerResult(Value pages)
        {
            string url;
            var myUri = new Uri(pages.url);

            if (myUri.Host == "www.bing.com" && myUri.AbsolutePath == "/cr")
            {
                url = HttpUtility.ParseQueryString(myUri.Query).Get("r");
            }
            else
            {
                url = pages.url;
            }

            var zummerResult = new ZummerSearchResult
            {
                Name = pages.name,
                Snippet = pages.snippet,
                Url = url
            };

            return zummerResult;
        }
    }
}