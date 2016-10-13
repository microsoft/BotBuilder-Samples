namespace Search.Services
{
    using System.Threading.Tasks;
    using Models;
    using System.Collections.Generic;

    public interface ISearchClient
    {
        Task<GenericSearchResult> SearchAsync(SearchQueryBuilder queryBuilder, string refiner = null);
        IDictionary<string, SearchField> Schema { get; }
    }
}
