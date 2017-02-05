using System.Threading.Tasks;
using Zummer.Models.Search;

namespace Zummer.Services
{
    public interface ISearchService
    {
        Task<BingSearch> FindArticles(string query);
    }
}
