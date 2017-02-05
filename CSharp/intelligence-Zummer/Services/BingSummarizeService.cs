using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Zummer.Models.Summarize;
using System.Configuration;

namespace Zummer.Services
{
    /// <summary>
    /// Responsible for calling Bing Summarizer API
    /// </summary>
    internal sealed class BingSummarizeService : ISummarizeService
    {
        private const string BingSummarizeEndpoint = "https://cognitivegarage.azure-api.net/bingSummarizer/summary";

        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>
        {
            { "Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["BingSummarizeSerivceKey"] }
        };

        private readonly IApiHandler apiHandler;

        public BingSummarizeService(IApiHandler apiHandler)
        {
            SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler);
        }

        public async Task<BingSummarize> GetSummary(string url)
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "url", url }
            };

            return await this.apiHandler.GetJsonAsync<BingSummarize>(BingSummarizeEndpoint, requestParameters, Headers);
        }
    }
}