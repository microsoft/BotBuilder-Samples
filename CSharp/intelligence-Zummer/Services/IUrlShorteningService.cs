using System.Threading.Tasks;

namespace Zummer.Services
{
    public interface IUrlShorteningService
    {
        Task<string> GetShortenedUrl(string url);
    }
}