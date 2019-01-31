using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;

namespace Asp_Mvc_Bot
{
    public interface IBotFrameworkAdapter
    {
        Task ProcessAsync(HttpRequest request, HttpResponse response, BotCallbackHandler callback, CancellationToken cancellationToken = default(CancellationToken));
    }
}
