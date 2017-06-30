using System.Threading.Tasks;
using Search.Models;

namespace Search.Services
{
    public interface ISearchClient
    {
        SearchSchema Schema { get; }
        Task<GenericSearchResult> SearchAsync(SearchSpec spec, string refiner = null);
    }
}