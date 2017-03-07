using System.Threading.Tasks;
using NewsieBot.Models.Summarize;

namespace NewsieBot.Services
{
    public interface ISummarizeService
    {
        Task<BingSummarize> GetSummary(string url);
    }
}
