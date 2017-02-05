using System.Threading.Tasks;
using Zummer.Models.Summarize;

namespace Zummer.Services
{
    public interface ISummarizeService
    {
        Task<BingSummarize> GetSummary(string url);
    }
}
