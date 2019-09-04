// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams.Middlewares;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class TeamsAdapter : BotFrameworkHttpAdapter
    {
        private UserState _userState;

        public TeamsAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, UserState userState)
            : base(configuration, logger)
        {
            _userState = userState;
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };

         

            Use(new TeamsMiddleware(new ConfigurationCredentialProvider(configuration)));
        }

        public override async Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {

            var id =  await base.SendActivitiesAsync(turnContext, activities, cancellationToken);
            if (id != null)
            {
                var idAccessor = _userState.CreateProperty<string>("lastMessageId");

                await idAccessor.SetAsync(turnContext, id.Last().Id);
            }

            return id;
        }
    }
}
