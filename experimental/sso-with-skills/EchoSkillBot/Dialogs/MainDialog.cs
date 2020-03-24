using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.EchoSkillBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Logger = logger;

            AddDialog(new SignInDialog(configuration));
            AddDialog(new SignOutDialog(configuration));
            AddDialog(new DisplayTokenDialog(configuration));
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();
                DialogTurnResult dialogResult;

                text = text.Replace("skill ", string.Empty).Replace("skill: ", string.Empty); ;

                // Top level commands
                if (text == "signin" || text == "login" || text == "sign in" || text == "log in")
                {
                    dialogResult = await innerDc.BeginDialogAsync(nameof(SignInDialog), null, cancellationToken);
                }
                else if (text == "signout" || text == "logout" || text == "sign out" || text == "log out")
                {
                    dialogResult = await innerDc.BeginDialogAsync(nameof(SignOutDialog), null, cancellationToken);
                }
                else if (text == "token" || text == "get token" || text == "gettoken")
                {
                    dialogResult = await innerDc.BeginDialogAsync(nameof(DisplayTokenDialog), null, cancellationToken);
                }
                else
                {
                    await innerDc.Context.SendActivityAsync(MessageFactory.Text($"Skill echo: {text}"), cancellationToken);
                    dialogResult = new DialogTurnResult(DialogTurnStatus.Complete);
                }

                return dialogResult;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }
    }
}
