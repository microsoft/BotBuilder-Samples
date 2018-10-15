using Botv4AzureSearch.Models;
using Botv4AzureSearch.Services;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Botv4AzureSearch.Dialogs
{
    public class StartDialog : IBot
    {
        private static SearchIndexClient indexClient;
        private IConfiguration configuration;
        private string errorMessage;

        public StartDialog(IConfiguration config)
        {
            // Azure Search definition
            this.configuration = config;
            indexClient = AzureSearchService.CreateSearchIndexClient(configuration["SearchServiceName"], configuration["SearchDialogsIndexName"], configuration["SearchServiceQueryApiKey"]);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type is ActivityTypes.Message)
            {
                try
                {
                    await QueryOnAzureSearch(turnContext);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message.ToString();
                }
            }
        }

        private static async Task QueryOnAzureSearch(ITurnContext turnContext)
        {
            SearchParameters parameters = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                Top = 1,
                Filter = null,
            };

            // Get the documents through Azure search using the AzureSearchService service class
            DocumentSearchResult<Squad> searchResults = AzureSearchService.RunQuery(turnContext.Activity.Text, indexClient, parameters);

            foreach (var res in searchResults.Results)
            {
                await turnContext.SendActivityAsync($"Squad Paragpraph: {res.Document.paragraph_text}");
            }
        }
    }
}
