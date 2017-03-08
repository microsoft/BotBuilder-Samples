using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Zummer.Models.Search;

namespace Zummer.Services
{
    /// <summary>
    /// Responsible for calling Bing Web Search API
    /// </summary>
    internal sealed class BingSearchService : ISearchService
    {
        private const string BingSearchEndpoint = "https://api.cognitive.microsoft.com/bing/v5.0/search/";

        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>
        {
            { "Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["BingSearchServiceKey"] }
        }; 

        private readonly IApiHandler apiHandler;

        public BingSearchService(IApiHandler apiHandler)
        {
            SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler);
        }

        public async Task<BingSearch> FindArticles(string query)
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "q", $"{query} site:wikipedia.org" },
                { "form", "BTCSWR" }
            };

            return await this.apiHandler.GetJsonAsync<BingSearch>(BingSearchEndpoint, requestParameters, Headers);
        }
    }
}