using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Newsie.Models.News;

namespace Newsie.Services
{
    /// <summary>
    /// Responsible for calling Bing News Search API
    /// </summary>
    internal sealed class BingNewsService : INewsService
    {
        private const string BingNewsEndpoint = "https://api.cognitive.microsoft.com/bing/v5.0/news/";

        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>
        {
            { "Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["BingNewsSerivceKey"] }
        }; 

        private readonly IApiHandler apiHandler;

        public BingNewsService(IApiHandler apiHandler)
        {
            SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler);
        }

        public async Task<BingNews> FindNewsByQuery(string query)
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "q", query }
            };

            return await this.apiHandler.GetJsonAsync<BingNews>(BingNewsEndpoint + "search", requestParameters, Headers);
        }

        public async Task<BingNews> FindNewsByCategory(string categoryName)
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "category", categoryName }
            };

            return await this.apiHandler.GetJsonAsync<BingNews>(BingNewsEndpoint, requestParameters, Headers);
        }
    }
}