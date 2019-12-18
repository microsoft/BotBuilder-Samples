// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.Dialogs
{
    public class OAuthTestDialog : CancelAndHelpDialog
    {
        private readonly string _connectionName;

        public OAuthTestDialog(IConfiguration configuration)
            : base(nameof(OAuthTestDialog))
        {
            _connectionName = configuration["ConnectionName"];

            AddDialog(new OAuthPrompt(
                nameof(OAuthPrompt),
                new OAuthPromptSettings
                {
                    ConnectionName = _connectionName,
                    Text = $"Please Sign In to connection: '{_connectionName}'",
                    Title = "Sign In",
                    Timeout = 300000 // User has 5 minutes to login (1000 * 60 * 5)
                }));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] { PromptStepAsync, LoginStepAsync }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
        }

        private async Task<DialogTurnResult> LoginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the token from the previous step.
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                // Show the token
                var loggedInMessage = "You are now logged in.";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(loggedInMessage, loggedInMessage, InputHints.IgnoringInput), cancellationToken);
                var showTokenMessage = "Here is your token:";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{showTokenMessage} {tokenResponse.Token}", showTokenMessage, InputHints.IgnoringInput), cancellationToken);

                // Sign out
                var botAdapter = (BotFrameworkAdapter)stepContext.Context.Adapter;
                await botAdapter.SignOutUserAsync(stepContext.Context, _connectionName, null, cancellationToken);
                var signOutMessage = "I have signed you out.";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(signOutMessage, signOutMessage, inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            var tryAgainMessage = "Login was not successful please try again.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(tryAgainMessage, tryAgainMessage, InputHints.IgnoringInput), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
