namespace Search.Azure.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;
    using Search.Models;
    using Search.Services;

    public class AzureSearchClient : ISearchClient
    {
        private readonly ISearchIndexClient searchClient;
        private readonly IMapper<DocumentSearchResult, GenericSearchResult> mapper;

        public AzureSearchClient(IMapper<DocumentSearchResult, GenericSearchResult> mapper)
        {
            this.mapper = mapper;
            SearchServiceClient client = new SearchServiceClient(
                ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                new SearchCredentials(ConfigurationManager.AppSettings["SearchDialogsServiceKey"]));
            this.searchClient = client.Indexes.GetClient(ConfigurationManager.AppSettings["SearchDialogsIndexName"]);
        }

        public async Task<GenericSearchResult> SearchAsync(SearchQueryBuilder queryBuilder, string refiner)
        {
           var documentSearchResult = await this.searchClient.Documents.SearchAsync(queryBuilder.SearchText, BuildParameters(queryBuilder, refiner));

            return this.mapper.Map(documentSearchResult);
        }

        private static SearchParameters BuildParameters(SearchQueryBuilder queryBuilder, string facet)
        {
            SearchParameters parameters = new SearchParameters
            {
                Top = queryBuilder.HitsPerPage,
                Skip = queryBuilder.PageNumber * queryBuilder.HitsPerPage,
                SearchMode = SearchMode.All
            };

            if (facet != null)
            {
                parameters.Facets = new List<string> { facet };
            }

            if (queryBuilder.Refinements.Count > 0)
            {
                StringBuilder filter = new StringBuilder();
                string separator = string.Empty;

                foreach (var entry in queryBuilder.Refinements)
                {
                    foreach (string value in entry.Value)
                    {
                        filter.Append(separator);
                        filter.Append($"{entry.Key} eq '{EscapeFilterString(value)}'");
                        separator = " and ";
                    }
                }

                parameters.Filter = filter.ToString();
            }

            return parameters;
        }

        private static string EscapeFilterString(string s)
        {
            return s.Replace("'", "''");
        }
    }
}
