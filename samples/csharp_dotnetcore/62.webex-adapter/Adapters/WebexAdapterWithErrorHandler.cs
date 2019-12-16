using Microsoft.Bot.Builder.Adapters.Webex;
using Microsoft.Extensions.Configuration;

namespace WebexAdapterBot.Adapters
{
    public class WebexAdapterWithErrorHandler : WebexAdapter
    {
        public WebexAdapterWithErrorHandler(IConfiguration configuration)
            : base(configuration)
        {
            OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync("Sorry, something went wrong");
            };
        }
    }
}
