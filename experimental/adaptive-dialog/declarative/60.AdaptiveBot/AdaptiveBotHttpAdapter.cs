using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Dialogs.Declarative.Types;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class AdaptiveBotHttpAdapter : BotFrameworkHttpAdapter
    {
        public AdaptiveBotHttpAdapter(ICredentialProvider credentialProvider,
            IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger,
            IStorage storage, UserState userState, ConversationState conversationState, ResourceExplorer resourceExplorer)
            : base(credentialProvider)
        {
            this.Use(new RegisterClassMiddleware<IConfiguration>(configuration));
            this.UseStorage(storage);
            this.UseState(userState, conversationState);
            this.UseResourceExplorer(resourceExplorer);
            this.UseLanguageGeneration(resourceExplorer);
            this.UseAdaptiveDialogs();
            this.UseDebugger(configuration.GetValue<int>("debugport", 4712), events: new Events<AdaptiveEvents>());

            this.OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
                await turnContext.SendActivityAsync(exception.Message).ConfigureAwait(false);

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }
            };
        }
    }
}
