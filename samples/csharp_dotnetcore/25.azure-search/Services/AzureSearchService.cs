using Botv4AzureSearch.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botv4AzureSearch.Services
{
    public class AzureSearchService
    {
        public static SearchIndexClient CreateSearchIndexClient(string searchServiceName, string indexName, string queryApiKey)
        {
            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));

            return indexClient;
        }

        public static DocumentSearchResult<Squad> RunQuery(string searchString, SearchIndexClient indexClient, SearchParameters parameters)
        {
            try
            {
                DocumentSearchResult<Squad> searchResults = indexClient.Documents.Search<Squad>(searchString, parameters);
                return searchResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }

            return null;
        }
    }
}
