using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebexAdapterBot.Adapters
{
    public class BotFrameworkAdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public BotFrameworkAdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                await turnContext.SendActivityAsync("Sorry, something went wrong");
            };

        }
    }
}
