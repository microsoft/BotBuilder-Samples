namespace SimpleTaskAutomationBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    #pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string ChangePasswordOption = "Change Password";
        private const string ResetPasswordOption = "Reset Password";
        
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
                context, 
                this.AfterChoiceSelected, 
                new[] { ChangePasswordOption, ResetPasswordOption }, 
                "What do you want to do today?", 
                "I am sorry but I didn't understand that. I need you to select one of the options below",
                attempts: 2);
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                switch (selection)
                {
                    case ChangePasswordOption:
                        await context.PostAsync("This functionality is not yet implemented! Try resetting your password.");
                        await this.StartAsync(context);
                        break;

                    case ResetPasswordOption:
                        context.Call(new ResetPasswordDialog(), this.AfterResetPassword);
                        break;
                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterResetPassword(IDialogContext context, IAwaitable<bool> result)
        {
            var success = await result;

            if (!success)
            {
                await context.PostAsync("Your identity was not verified and your password cannot be reset");
            }

            await this.StartAsync(context);
        }
    }
}