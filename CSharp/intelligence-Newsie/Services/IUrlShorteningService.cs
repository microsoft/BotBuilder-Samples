using System.Threading.Tasks;

namespace NewsieBot.Services
{
    public interface IUrlShorteningService
    {
        Task<string> GetShortenedUrl(string url);
    }
}