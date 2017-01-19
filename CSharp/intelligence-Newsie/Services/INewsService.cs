using System.Threading.Tasks;
using NewsieBot.Models.News;

namespace NewsieBot.Services
{
    public interface INewsService
    {
        Task<BingNews> FindNewsByCategory(string categoryName);

        Task<BingNews> FindNewsByQuery(string query);
    }
}
