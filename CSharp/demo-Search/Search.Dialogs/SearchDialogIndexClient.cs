using System.Configuration;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Microsoft.Bot.Sample.SearchDialogs
{
    // TODO: dependency-inject a SearchIndexClient instance instead of this
    // static wrapper to give callers control over the search client
    public static class SearchDialogIndexClient
    {
        private static readonly ISearchIndexClient searchClient;
        private static SearchSchema schema;

        static SearchDialogIndexClient()
        {
            var indexName = ConfigurationManager.AppSettings["SearchDialogsIndexName"];
            var adminKey = ConfigurationManager.AppSettings["SearchDialogsServiceAdminKey"];
            if (adminKey != null)
            {
                var adminClient = new SearchServiceClient(ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                                                                      new SearchCredentials(adminKey));
                schema = new SearchSchema().AddFields(adminClient.Indexes.Get(indexName).Fields);
            }
            var client = new SearchServiceClient(ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                                                                 new SearchCredentials(ConfigurationManager.AppSettings["SearchDialogsServiceKey"]));
            searchClient = client.Indexes.GetClient(indexName);
        }

        public static ISearchIndexClient Client
        {
            get { return searchClient; }
        }

        public static SearchSchema Schema
        {
            get { return schema; }
            set { schema = value; }
        }
   }
}
