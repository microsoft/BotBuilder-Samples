// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
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
    /// </summary>
    [Route("bot1")]
    public class Bot1Controller : BotControllerBase
    {
        public Bot1Controller(BotConfiguration botConfig)
            : base(botConfig, "bot1 development")
        {
            Options.OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync("Sorry, it looks like something went wrong in bot1.");
            };
        }

        protected override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo '{turnContext.Activity.Text}' from bot 1."));
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded.Any())
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome {member.Name} to ASP MVC Bot 1."), cancellationToken);
                }
            }
        }
    }
}
