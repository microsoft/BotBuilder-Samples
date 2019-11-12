using Microsoft.Bot.Builder.Adapters.Slack;
using Microsoft.Extensions.Configuration;

namespace SlackAdapterBot.Adapters
{
    public class SlackAdapterWithErrorHandler : SlackAdapter
    {
        public SlackAdapterWithErrorHandler(IConfiguration configuration)
            : base(configuration)
        {
            OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync("Sorry, something went wrong");
            };
        }
    }
}
