// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Bot.Builder.Community.Dialogs.FormFlow;
using ContosoHelpdeskChatBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContosoHelpdeskChatBot.Dialogs
{
    public class ResetPasswordDialog : ComponentDialog
    {
        public ResetPasswordDialog()
            : base(nameof(ResetPasswordDialog))
        {
            InitialDialogId = nameof(WaterfallDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                BeginFormflowAsync,
                ProcessRequestAsync,
            }));
            AddDialog(FormDialog.FromForm(BuildResetPasswordForm, FormOptions.PromptInStart));
        }

        private async Task<DialogTurnResult> BeginFormflowAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await stepContext.Context.SendActivityAsync("Alright I will help you create a temp password.");

            // Check the passcode and fail out or begin the Formflow dialog.
            if (SendPassCode(stepContext))
            {
                return await stepContext.BeginDialogAsync(
                    nameof(ResetPasswordPrompt),
                    cancellationToken: cancellationToken);
            }
            else
            {
                //here we can simply fail the current dialog because we have root dialog handling all exceptions
                throw new Exception("Failed to send SMS. Make sure email & phone number has been added to database.");
            }
        }

        private bool SendPassCode(DialogContext context)
        {
            //bool result = false;

            //Recipient Id varies depending on channel
            //refer ChannelAccount class https://docs.botframework.com/en-us/csharp/builder/sdkreference/dd/def/class_microsoft_1_1_bot_1_1_connector_1_1_channel_account.html#a0b89cf01fdd73cbc00a524dce9e2ad1a
            //as well as Activity class https://docs.botframework.com/en-us/csharp/builder/sdkreference/dc/d2f/class_microsoft_1_1_bot_1_1_connector_1_1_activity.html
            //int passcode = new Random().Next(1000, 9999);
            //Int64? smsNumber = 0;
            //string smsMessage = "Your Contoso Pass Code is ";
            //string countryDialPrefix = "+1";

            // TODO: save PassCode to database
            //using (var db = new ContosoHelpdeskContext())
            //{
            //    var reset = db.ResetPasswords.Where(r => r.EmailAddress == email).ToList();
            //    if (reset.Count >= 1)
            //    {
            //        reset.First().PassCode = passcode;
            //        smsNumber = reset.First().MobileNumber;
            //        result = true;
            //    }

            //    db.SaveChanges();
            //}

            // TODO: send passcode to user via SMS.
            //if (result)
            //{
            //    result = Helper.SendSms($"{countryDialPrefix}{smsNumber.ToString()}", $"{smsMessage} {passcode}");
            //}

            //return result;
            return true;
        }

        private IForm<ResetPasswordPrompt> BuildResetPasswordForm()
        {
            return new FormBuilder<ResetPasswordPrompt>()
                .Field(nameof(ResetPasswordPrompt.PassCode))
                .Build();
        }

        private async Task<DialogTurnResult> ProcessRequestAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the result from the Formflow dialog when it ends.
            if (stepContext.Reason != DialogReason.CancelCalled)
            {
                var prompt = stepContext.Result as ResetPasswordPrompt;
                int? passcode;

                // TODO: Retrieve the passcode from the database.
                passcode = 1111;

                if (prompt.PassCode == passcode)
                {
                    string temppwd = "TempPwd" + new Random().Next(0, 5000);
                    await stepContext.Context.SendActivityAsync(
                        $"Your temp password is {temppwd}",
                        cancellationToken: cancellationToken);
                }
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
