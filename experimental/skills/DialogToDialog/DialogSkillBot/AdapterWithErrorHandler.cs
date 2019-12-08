// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.DialogSkillBot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<AdapterWithErrorHandler> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Send a catch-all apology to the user.
                var errorMessage = $"Skill Error, it looks like something went wrong.\r\n{exception}";
                await turnContext.SendActivityAsync(MessageFactory.Text(errorMessage, "Something went wrong in the skill.", InputHints.IgnoringInput));
            };
        }
    }
}
