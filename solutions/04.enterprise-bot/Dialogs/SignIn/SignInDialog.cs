// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using EnterpriseBot.Dialogs.SignIn.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Graph;

namespace EnterpriseBot
{
    public class SignInDialog : ComponentDialog
    {
        // Constants
        public const string LoginPrompt = "loginPrompt";

        // Fields
        private static SignInResponses _responder = new SignInResponses();

        public SignInDialog(string connectionName)
            : base(nameof(SignInDialog))
        {
            InitialDialogId = nameof(SignInDialog);
            ConnectionName = connectionName;

            var authenticate = new WaterfallStep[]
            {
                AskToLogin,
                FinishAuthDialog,
            };

            AddDialog(new WaterfallDialog(InitialDialogId, authenticate));
            AddDialog(new OAuthPrompt(LoginPrompt, new OAuthPromptSettings()
            {
                ConnectionName = ConnectionName,
                Title = SignInStrings.TITLE,
                Text = SignInStrings.PROMPT,
            }));
        }

        private string ConnectionName { get; set; }

        private async Task<DialogTurnResult> AskToLogin(WaterfallStepContext sc, CancellationToken cancellationToken) => await sc.PromptAsync(LoginPrompt, new PromptOptions());

        private async Task<DialogTurnResult> FinishAuthDialog(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            var activity = sc.Context.Activity;
            if (sc.Result != null)
            {
                var tokenResponse = sc.Result as TokenResponse;

                if (tokenResponse?.Token != null)
                {
                    var user = await GetProfile(sc.Context, tokenResponse);
                    await _responder.ReplyWith(sc.Context, SignInResponses.Succeeded, new { name = user.DisplayName });
                    return await sc.EndDialogAsync(tokenResponse);
                }
            }
            else
            {
                await _responder.ReplyWith(sc.Context, SignInResponses.Failed);
            }

            return await sc.EndDialogAsync();
        }

        private async Task<User> GetProfile(ITurnContext context, TokenResponse tokenResponse)
        {
            var token = tokenResponse;
            var client = new GraphClient(token.Token);

            return await client.GetMe();
        }
    }
}
