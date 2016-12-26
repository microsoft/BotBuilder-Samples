using System.Threading.Tasks;

namespace Newsie.Services
{
    public interface IUrlShorteningService
    {
        Task<string> GetShortenedUrl(string url);
    }
}