// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bot.Builder.Community.Dialogs.FormFlow;
using ContosoHelpdeskChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace ContosoHelpdeskChatBot.Dialogs
{
    public class LocalAdminDialog : ComponentDialog
    {
        public LocalAdminDialog() : base(nameof(LocalAdminDialog))
        {
            InitialDialogId = nameof(WaterfallDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                BeginFormflowAsync,
                SaveResultAsync,
            }));
            AddDialog(FormDialog.FromForm(BuildLocalAdminForm, FormOptions.PromptInStart));
        }

        private async Task<DialogTurnResult> BeginFormflowAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await stepContext.Context.SendActivityAsync("Great I will help you request local machine admin.");

            // Begin the Formflow dialog.
            return await stepContext.BeginDialogAsync(
                nameof(LocalAdminPrompt),
                cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> SaveResultAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the result from the Formflow dialog when it ends.
            if (stepContext.Reason != DialogReason.CancelCalled)
            {
                var admin = stepContext.Result as LocalAdminPrompt;

                //TODO: Save to this information to the database.
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        // Nearly the same as before.
        private IForm<LocalAdminPrompt> BuildLocalAdminForm()
        {
            // Here's an example of how validation can be used with FormBuilder.
            return new FormBuilder<LocalAdminPrompt>()
                .Field(nameof(LocalAdminPrompt.MachineName),
                validate: async (state, value) =>
                {
                    var result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    //this.admin.MachineName = (string)value;
                    return result;
                })
                .Field(nameof(LocalAdminPrompt.AdminDuration),
                validate: async (state, value) =>
                {
                    var result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    //this.admin.AdminDuration = Convert.ToInt32((long)value) as int?;
                    return result;
                })
                .Build();
        }
    }
}
