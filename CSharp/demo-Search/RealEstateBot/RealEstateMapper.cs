namespace RealEstateBot
{
    using System.Linq;
    using Microsoft.Azure.Search.Models;
    using Search.Azure.Services;
    using Search.Models;

    public class RealEstateMapper : IMapper<DocumentSearchResult, GenericSearchResult>
    {
        public GenericSearchResult Map(DocumentSearchResult documentSearchResult)
        {
            var searchResult = new GenericSearchResult();

            searchResult.Results = documentSearchResult.Results.Select(r => ToSearchHit(r)).ToList();
            searchResult.Facets = documentSearchResult.Facets?.ToDictionary(kv => kv.Key, kv => kv.Value.Select(f => ToFacet(f)));

            return searchResult;
        }

        private static GenericFacet ToFacet(FacetResult facetResult)
        {
            return new GenericFacet
            {
                Value = facetResult.Value,
                Count = facetResult.Count.Value
            };
        }

        private static SearchHit ToSearchHit(SearchResult hit)
        {
            return new SearchHit
            {
                Key = (string)hit.Document["listingId"],
                Title = GetTitleForItem(hit),
                PictureUrl = (string)hit.Document["thumbnail"],
                Description = (string)hit.Document["description"]
            };
        }

        private static string GetTitleForItem(SearchResult result)
        {
            var beds = result.Document["beds"];
            var baths = result.Document["baths"];
            var city = result.Document["city"];
            var price = result.Document["price"];

            return $"{beds} bedroom, {baths} bath in {city}, ${price:#,0}";
        }
    }
}
