using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Community.Dialogs.FormFlow;
using ContosoHelpdeskChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace ContosoHelpdeskChatBot.Dialogs
{
    public class LocalAdminDialog : ComponentDialog
    {
        // Set up our dialog and prompt IDs as constants.
        private const string MainDialog = "mainDialog";
        private static string AdminDialog { get; } = nameof(LocalAdminPrompt);

        public LocalAdminDialog(string dialogId) : base(dialogId)
        {
            InitialDialogId = MainDialog;

            AddDialog(new WaterfallDialog(MainDialog, new WaterfallStep[]
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
            return await stepContext.BeginDialogAsync(AdminDialog, cancellationToken: cancellationToken);
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
            //here's an example of how validation can be used in form builder
            return new FormBuilder<LocalAdminPrompt>()
                .Field(nameof(LocalAdminPrompt.MachineName),
                validate: async (state, value) =>
                {
                    ValidateResult result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    //this.admin.MachineName = (string)value;
                    return result;
                })
                .Field(nameof(LocalAdminPrompt.AdminDuration),
                validate: async (state, value) =>
                {
                    ValidateResult result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    //this.admin.AdminDuration = Convert.ToInt32((long)value) as int?;
                    return result;
                })
                .Build();
        }
    }
}
