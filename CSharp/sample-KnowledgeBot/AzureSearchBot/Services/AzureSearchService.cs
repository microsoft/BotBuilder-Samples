using System.Net.Http;
using System.Web.Configuration;
using System.Threading.Tasks;
using AzureSearchBot.Model;
using Newtonsoft.Json;
using System;

namespace AzureSearchBot.Services
{
    [Serializable]
    public class AzureSearchService
    {
        private static readonly string QueryString = $"https://{WebConfigurationManager.AppSettings["SearchName"]}.search.windows.net/indexes/{WebConfigurationManager.AppSettings["IndexName"]}/docs?api-key={WebConfigurationManager.AppSettings["SearchKey"]}&api-version=2015-02-28&";

        public async Task<SearchResult> SearchByName(string name)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}search={name}";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }

        public async Task<FacetResult> FetchFacets()
        {
            using (var httpClient = new HttpClient())
            {
                string facetQuery = $"{QueryString}facet=Era";
                string response = await httpClient.GetStringAsync(facetQuery);
                return JsonConvert.DeserializeObject<FacetResult>(response);
            }
        }

        public async Task<SearchResult> SearchByEra(string era)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}$filter=Era eq '{era}'";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }
    }
}