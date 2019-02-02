using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;

namespace Asp_Mvc_Bot
{
    public interface IBotFrameworkAdapter
    {
        Task ProcessAsync(HttpRequest request, HttpResponse response, IBot bot, CancellationToken cancellationToken = default(CancellationToken));
    }
}
