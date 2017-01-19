using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace NewsieBot.Services
{
    internal sealed class TinyUrlRequestHandler : IUrlShorteningService
    {
        private const string TinyUrlEndpoint = "http://tinyurl.com/api-create.php";

        private readonly IApiHandler apiHandler;

        public TinyUrlRequestHandler(IApiHandler apiHandler)
        {
            SetField.NotNull(out this.apiHandler, nameof(apiHandler), apiHandler);
        }

        public async Task<string> GetShortenedUrl(string url)
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "url", HttpUtility.UrlEncode(url) }
            };

            return await this.apiHandler.GetStringAsync(TinyUrlEndpoint, requestParameters);
        }
    }
}