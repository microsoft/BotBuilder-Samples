namespace JobListingBot
{
    using System.Linq;
    using Microsoft.Azure.Search.Models;
    using Search.Azure.Services;
    using Search.Models;

    public class JobsMapper : IMapper<DocumentSearchResult, GenericSearchResult>
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

        private static SearchHit ToSearchHit(SearchResult searchResult)
        {
            var searchHit = new SearchHit
            {
                Key = (string)searchResult.Document["id"],
                Title = GetTitleForItem(searchResult),
                PictureUrl = null,
                Description = ((string)searchResult.Document["job_description"]).Substring(0, 512) + "...",
            };

            object minimumRequirements;

            if (searchResult.Document.TryGetValue("minimum_qual_requirements", out minimumRequirements))
            {
                searchHit.PropertyBag.Add("minimum_qual_requirements", minimumRequirements);
            }

            object preferedSkills;

            if (searchResult.Document.TryGetValue("preferred_skills", out preferedSkills))
            {
                searchHit.PropertyBag.Add("preferred_skills", preferedSkills);
            }

            return searchHit;
        }

        private static string GetTitleForItem(SearchResult result)
        {
            var businessTitle = result.Document["business_title"];
            var agency = result.Document["agency"];
            var salaryRangeFrom = result.Document["salary_range_from"];
            var salaryRangeTo = result.Document["salary_range_to"];

            return $"{businessTitle} at {agency}, {salaryRangeFrom:C0} to {salaryRangeTo:C0}";
        }
    }
}
