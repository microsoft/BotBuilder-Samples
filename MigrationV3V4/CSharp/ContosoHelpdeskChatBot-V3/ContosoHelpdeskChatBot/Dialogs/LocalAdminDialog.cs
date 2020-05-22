using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ContosoHelpdeskChatBot.Models;
using Microsoft.Bot.Builder.FormFlow;

namespace ContosoHelpdeskChatBot.Dialogs
{
    [Serializable]
    public class LocalAdminDialog : IDialog<object>
    {
        private LocalAdmin admin = new LocalAdmin();
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Great I will help you request local machine admin.");

            var localAdminDialog = FormDialog.FromForm(this.BuildLocalAdminForm, FormOptions.PromptInStart);

            context.Call(localAdminDialog, this.ResumeAfterLocalAdminFormDialog);
        }

        private async Task ResumeAfterLocalAdminFormDialog(IDialogContext context, IAwaitable<LocalAdminPrompt> userReply)
        {
            
            using (var db = new ContosoHelpdeskContext())
            {
                db.LocalAdmins.Add(admin);
                db.SaveChanges();
            }

            context.Done<object>(null);
        }

        private IForm<LocalAdminPrompt> BuildLocalAdminForm()
        {
            //here's an example of how validation can be used in form builder
            return new FormBuilder<LocalAdminPrompt>()
                .Field(nameof(LocalAdminPrompt.MachineName),
                validate: async (state, value) =>
                {
                    var result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    this.admin.MachineName = (string)value;
                    return result;
                })
                .Field(nameof(LocalAdminPrompt.AdminDuration),
                validate: async (state, value) =>
                {
                    var result = new ValidateResult { IsValid = true, Value = value };
                    //add validation here

                    this.admin.AdminDuration = Convert.ToInt32((long)value) as int?;
                    return result;
                })
                .Build();
        }
    }
}
