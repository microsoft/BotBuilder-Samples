using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bot.Helpers;
using Bot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Bot.Dialogs
{
    public class EmailDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(BotConstants.AskEmailAddress);
            context.Wait(OnMessageReceivedAsync);
        }


        private static async Task OnMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;
            var emailInput = message.Text;
            await OnHandleEmailInput(context, emailInput);
        }

        private static async Task OnHandleEmailInput(IDialogContext context, string emailInput)
        {
            var emailInputCount = context.UserData.Get<int>(BotConstants.EmailInputCountKey);
            var user = context.UserData.Get<User>(BotConstants.UserKey);
            var notes = context.UserData.Get<string>(BotConstants.AllNotesAsString);
            if (emailInputCount < 0 || emailInputCount > 2)
            {
                OnResetEmailUserData(context);
                context.Done(false);
                return;
            }

            var isValidEmail =
                new EmailValidator().OnCheckIsValidEmail(emailInput.Replace(BotConstants.BotMention, string.Empty));
            if (isValidEmail)
            {
                notes = Regex.Replace(notes.Replace("*", string.Empty), @"[\n]{2,}", "\n");
                context.UserData.SetValue(BotConstants.EmailInputCountKey, -1);
                OnSendEmailFromO365(emailInput, user, notes);
                OnResetEmailUserData(context);
                await context.PostAsync("Your notes have been e-mailed.");
                context.Done(true);
                return;
            }

            var remainingTries = 2 - emailInputCount;
            if (remainingTries > 0)
            {
                await context.PostAsync(
                    $"**Tries remaining: {remainingTries}**\r\n\nPlease provide a valid email address.");
                context.UserData.SetValue(BotConstants.EmailInputCountKey, emailInputCount + 1);
                context.Wait(OnMessageReceivedAsync);
                return;
            }

            context.UserData.SetValue(BotConstants.EmailInputCountKey, -1);
            await context.PostAsync(
                "**Try exporting again.**\r\n\nSorry, you exceed the maximum number of tries to provide the email.");
            OnResetEmailUserData(context);
            context.Done(false);
        }

        private static void OnSendEmailFromO365(string emailId, User user, string allNotes)
        {
            var strippedEmail = Regex.Replace(emailId, "<.*?>", string.Empty);
            using (var msg = new MailMessage())
            {
                msg.To.Add(new MailAddress(strippedEmail, user.UserName));
                msg.From = new MailAddress(BotConstants.BotEmailId);
                msg.Subject = BotConstants.EmailSubject;
                msg.Body = allNotes;
                msg.IsBodyHtml = false;
                var client = new SmtpClient
                {
                    Host = BotConstants.O365SmtpUrl,
                    Credentials = new NetworkCredential(BotConstants.BotEmailId, BotConstants.BotEmailPwd),
                    Port = 587,
                    EnableSsl = true
                };

                client.Send(msg);
                msg.Dispose();
            }
        }

        private static void OnResetEmailUserData(IBotData context)
        {
            context.UserData.SetValue(BotConstants.EmailInputCountKey, -1);
            context.UserData.SetValue(BotConstants.UserKey, string.Empty);
            context.UserData.SetValue(BotConstants.AllNotesAsString, string.Empty);
        }
    }
}