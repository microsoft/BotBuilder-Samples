using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.SimpleRootBot.Dialogs
{
    //public class MainDialog : ComponentDialog
    //{
    //    public MainDialog(IConfiguration configuration)
    //        : base(nameof(MainDialog))
    //    {
    //        var connectionName = configuration.GetSection("ConnectionName")?.Value ?? throw new ArgumentNullException("Connection name is needed in configuration");

    //        var steps = new WaterfallStep[]
    //            {
    //                SignInStepAsync,
    //                ShowTokenResponseAsync
    //            };
    //        AddDialog(new WaterfallDialog(nameof(MainDialog), steps));
    //        AddDialog(new OAuthPrompt(
    //            nameof(OAuthPrompt),
    //            new OAuthPromptSettings()
    //            {
    //                ConnectionName = connectionName,
    //                Text = "Sign In to AAD",
    //                Title = "Sign In"
    //            }));
    //    }

    //    private async Task<DialogTurnResult> SignInStepAsync(WaterfallStepContext context, CancellationToken cancellationToken)
    //    {
    //        return await context.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken).ConfigureAwait(false);
    //    }

    //    private async Task<DialogTurnResult> ShowTokenResponseAsync(WaterfallStepContext context, CancellationToken cancellationToken)
    //    {
    //        var result = context.Result as TokenResponse;
    //        if (result == null)
    //        {
    //            await context.Context.SendActivityAsync(MessageFactory.Text("Skill: No token response from OAuthPrompt"));
    //        }
    //        else
    //        {
    //            await context.Context.SendActivityAsync(MessageFactory.Text($"Skill: Your token is {result.Token}"));
    //        }

    //        return await context.EndDialogAsync(null, cancellationToken);
    //    }
    //}
}
