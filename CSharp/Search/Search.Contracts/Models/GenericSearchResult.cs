namespace Search.Models
{
    using System.Collections.Generic;

    public class GenericSearchResult
    {
        public IEnumerable<SearchHit> Results { get; set; }

        public IDictionary<string, IEnumerable<GenericFacet>> Facets { get; set; }
    }
}