namespace Search.Services
{
    using System.Threading.Tasks;
    using Models;

    public interface ISearchClient
    {
        Task<GenericSearchResult> SearchAsync(SearchQueryBuilder queryBuilder, string refiner = null);
    }
}
