// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.SkillBot.Dialogs
{
    public class SignInDialog : ComponentDialog
    {
        string _connectionName;
        public SignInDialog(IConfiguration configuration)
            : base(nameof(SignInDialog))
        {
            _connectionName = configuration.GetSection("ConnectionName")?.Value;

            var steps = new WaterfallStep[] {
                SignInStepAsync,
                DisplayTokenAsync
            };

            AddDialog(new WaterfallDialog(nameof(SignInDialog), steps));
            AddDialog(new OAuthPrompt(nameof(OAuthPrompt), new OAuthPromptSettings() { ConnectionName = _connectionName, Text = "Sign In to AAD for the Skill", Title = "Sign In" }));
        }

        private async Task<DialogTurnResult> SignInStepAsync(WaterfallStepContext context, CancellationToken cancellationToken)
        {
            var userid = context.Context.Activity.From.Id;
            return await context.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayTokenAsync(WaterfallStepContext context, CancellationToken cancellationToken)
        {
            var result = context.Result as TokenResponse;

            if (result == null)
            {
                await context.Context.SendActivityAsync("No token was provided for the skill.");
            }
            else
            {
                await context.Context.SendActivityAsync($"Here is your token for the skill: {result.Token}");
            }

            return await context.EndDialogAsync(null, cancellationToken);
        }
    }
}
