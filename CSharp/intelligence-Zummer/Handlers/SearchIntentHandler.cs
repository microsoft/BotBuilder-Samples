using System;
using System.Linq;
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

namespace Zummer.Handlers
{
    internal sealed class SearchIntentHandler : IIntentHandler
    {
        private readonly ISearchService bingSearchService;
        private readonly ISummarizeService bingSummarizeService;
        private readonly IBotToUser botToUser;

        public SearchIntentHandler(IBotToUser botToUser, ISearchService bingSearchService, ISummarizeService bingSummarizeService)
        {
            SetField.NotNull(out this.bingSearchService, nameof(bingSearchService), bingSearchService);
            SetField.NotNull(out this.bingSummarizeService, nameof(bingSummarizeService), bingSummarizeService);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
        }

        public async Task Respond(IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            EntityRecommendation entityRecommendation;

            var query = result.TryFindEntity(ZummerStrings.ArticlesEntityTopic, out entityRecommendation)
                ? entityRecommendation.Entity
                : result.Query;

            await this.botToUser.PostAsync(string.Format(Strings.SearchTopicTypeMessage));

            var bingSearch = await this.bingSearchService.FindArticles(query);

            var zummerResult = this.PrepareZummerResult(query, bingSearch.webPages.value[0]);

            var bingSummary = await this.bingSummarizeService.GetSummary(zummerResult.Url);

            if (bingSummary?.Data != null && bingSummary.Data.Length != 0)
            {
                var summaryText = bingSummary.Data.Aggregate(
                    $"### [{zummerResult.Tile}]({zummerResult.Url})"
                    + "\n" +
                    $"**{Strings.SummaryString}**"
                    + "\n\n",
                    (current, datum) => current + (datum.Text + "\n\n"));

                summaryText +=
                    $"*{string.Format(Strings.PowerBy, $"[Bing™](https://www.bing.com/search/?q={zummerResult.Query} site:wikipedia.org)")}*";

                await this.botToUser.PostAsync(summaryText);
            }
            else
            {
                await this.botToUser.PostAsync(Strings.SummaryErrorMessage);
            }
        }

        private ZummerSearchResult PrepareZummerResult(string query, Value page)
        {
            string url;
            var myUri = new Uri(page.url);

            if (myUri.Host == "www.bing.com" && myUri.AbsolutePath == "/cr")
            {
                url = HttpUtility.ParseQueryString(myUri.Query).Get("r");
            }
            else
            {
                url = page.url;
            }

            var zummerResult = new ZummerSearchResult
            {
                Url = url,
                Query = query,
                Tile = page.name
            };

            return zummerResult;
        }
    }
}