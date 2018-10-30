// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Schema;

namespace Asp_Mvc_Bot.Controllers
{
    /// <summary>
    /// This is just a regular MVC Controller with the Bot Builder specific code moved into a base class.
    /// In this case the typical Activity type switching has also been moved into a common base class.
    /// </summary>
    [Route("bot3")]
    public class Bot3Controller : BotActivityControllerBase
    {
        public Bot3Controller(BotConfiguration botConfig)
            : base(botConfig, "bot3 development")
        {
            Options.OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync("Sorry, it looks like something went wrong in bot3.");
            };
        }

        protected override async Task OnMessageActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo '{turnContext.Activity.Text}' from bot 3."));
        }

        protected override async Task OnMemberAddedAsync(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome {member.Name} to ASP MVC Bot 3."), cancellationToken);
        }
    }
}
