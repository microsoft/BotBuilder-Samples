using Microsoft.Bot.Builder.Adapters.Facebook;
using Microsoft.Extensions.Configuration;

namespace FacebookAdapterBot.Adapters
{
    public class FacebookAdapterWithErrorHandler : FacebookAdapter
    {
        public FacebookAdapterWithErrorHandler(IConfiguration configuration)
            : base(configuration)
        {
            OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync("Sorry, something went wrong");
            };
        }
    }
}
