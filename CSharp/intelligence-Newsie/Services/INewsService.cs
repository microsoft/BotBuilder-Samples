using System.Threading.Tasks;
using Newsie.Models.News;

namespace Newsie.Services
{
    public interface INewsService
    {
        Task<BingNews> FindNewsByCategory(string categoryName);

        Task<BingNews> FindNewsByQuery(string query);
    }
}
