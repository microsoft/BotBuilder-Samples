namespace SimpleTaskAutomationBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;

    #pragma warning disable 1998

    [Serializable]
    public class ResetPasswordDialog : IDialog<bool>
    {
        private const string PhoneRegexPattern = @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

        public async Task StartAsync(IDialogContext context)
        {
            var promptPhoneDialog = new PromptStringRegex(
                "Please enter your phone number:", 
                PhoneRegexPattern, 
                "The value entered is not phone number. Please try again using the following format (xyz) xyz-wxyz:",
                "You have tried to enter your phone number many times. Please try again later.",
                attempts: 2);

            context.Call(promptPhoneDialog, this.ResumeAfterPhoneEntered);
        }

        private async Task ResumeAfterPhoneEntered(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var phone = await result;

                if (phone != null)
                {
                    await context.PostAsync($"The phone you provided is: {phone}");

                    var promptBirthDialog = new PromptDate(
                        "Please enter your date of birth (MM/dd/yyyy):",
                        "The value you entered is not a valid date. Please try again:",
                        "You have tried to enter your date of birth many times. Please try again later.",
                        attempts: 2);

                    context.Call(promptBirthDialog, this.AfterDateOfBirthEntered);
                }
                else
                {
                    context.Done(false);
                }
            }
            catch (TooManyAttemptsException)
            {
                context.Done(false);
            }
        }

        private async Task AfterDateOfBirthEntered(IDialogContext context, IAwaitable<DateTime> result)
        {
            try
            {
                var dateOfBirth = await result;

                if (dateOfBirth != DateTime.MinValue)
                {
                    await context.PostAsync($"The date of birth you provided is: {dateOfBirth.ToShortDateString()}");

                    // Add your custom reset password logic here.
                    var newPassword = Guid.NewGuid().ToString().Replace("-", string.Empty);

                    await context.PostAsync($"Thanks! Your new password is _{newPassword}_");

                    context.Done(true);
                }
                else
                {
                    context.Done(false);
                }
            }
            catch (TooManyAttemptsException)
            {
                context.Done(false);
            }
        }
    }
}
