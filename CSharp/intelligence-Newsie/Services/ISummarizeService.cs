using System.Threading.Tasks;
using Newsie.Models.Summarize;

namespace Newsie.Services
{
    public interface ISummarizeService
    {
        Task<BingSummarize> GetSummary(string url);
    }
}
