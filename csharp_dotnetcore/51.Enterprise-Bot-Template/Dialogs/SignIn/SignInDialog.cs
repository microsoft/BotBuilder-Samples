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
        public const string Name = "signIn";
        public const string LoginPrompt = "loginPrompt";

        // Fields
        private static SignInResponses _responder = new SignInResponses();

        public SignInDialog(string connectionName)
            : base(Name)
        {
            this.ConnectionName = connectionName;

            var authenticate = new WaterfallStep[]
            {
                this.AskToLogin,
                this.FinishAuthDialog,
            };

            this.AddDialog(new WaterfallDialog(Name, authenticate));
            this.AddDialog(new OAuthPrompt(LoginPrompt, new OAuthPromptSettings()
            {
                ConnectionName = ConnectionName,
                Title = SignInStrings.TITLE,
                Text = SignInStrings.PROMPT,
            }));
        }

        // Properties
        private string ConnectionName { get; set; }

        private async Task<DialogTurnResult> AskToLogin(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync(LoginPrompt, new PromptOptions());
        }

        private async Task<DialogTurnResult> FinishAuthDialog(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var activity = dc.Context.Activity;
            if (step.Result != null)
            {
                var tokenResponse = step.Result as TokenResponse;

                if (tokenResponse?.Token != null)
                {
                    var user = await this.GetProfile(dc.Context, tokenResponse);
                    await _responder.ReplyWith(dc.Context, SignInResponses.Succeeded, new { name = user.DisplayName });
                    return await dc.EndAsync(tokenResponse);
                }
            }
            else
            {
                await _responder.ReplyWith(dc.Context, SignInResponses.Failed);
            }

            return await dc.EndAsync();
        }

        private async Task<User> GetProfile(ITurnContext context, TokenResponse tokenResponse)
        {
            var token = tokenResponse;
            var client = new GraphClient(token.Token);

            return await client.GetMe();
        }
    }
}
