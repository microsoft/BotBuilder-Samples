namespace ContosoHelpdeskChatBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class RootDialog : ComponentDialog
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string InstallAppOption = "Install Application (install)";
        private const string ResetPasswordOption = "Reset Password (password)";
        private const string LocalAdminOption = "Request Local Admin (admin)";
        private const string GreetMessage = "Welcome to **Contoso Helpdesk Chat Bot**.\n\nI am designed to use with mobile email app, make sure your replies do not contain signatures. \n\nFollowing is what I can help you with, just reply with word in parenthesis:";
        private const string ErrorMessage = "Not a valid option";
        private static List<Choice> HelpdeskOptions = new List<Choice>()
            {
                new Choice(InstallAppOption) { Synonyms = new List<string>(){ "install" } },
                new Choice(ResetPasswordOption) { Synonyms = new List<string>(){ "password" } },
                new Choice(LocalAdminOption)  { Synonyms = new List<string>(){ "admin" } }
            };

        public RootDialog()
            : base(nameof(RootDialog))
        {
            AddDialog(new WaterfallDialog("choiceswaterfall", new WaterfallStep[]
            {
                PromptForOptionsAsync,
                ShowChildDialogAsync,
                ResumeAfterAsync,
            }));
            AddDialog(new InstallAppDialog(nameof(InstallAppDialog)));
            AddDialog(new LocalAdminDialog(nameof(LocalAdminDialog)));
            AddDialog(new ResetPasswordDialog(nameof(ResetPasswordDialog)));
            AddDialog(new ChoicePrompt("options"));
        }

        private async Task<DialogTurnResult> PromptForOptionsAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Prompt the user for a response using our choice prompt.
            return await stepContext.PromptAsync(
                "options",
                new PromptOptions()
                {
                    Choices = HelpdeskOptions,
                    Prompt = MessageFactory.Text(GreetMessage),
                    RetryPrompt = MessageFactory.Text(ErrorMessage)
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ShowChildDialogAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // string optionSelected = await userReply;
            string optionSelected = (stepContext.Result as FoundChoice).Value;

            switch (optionSelected)
            {
                case InstallAppOption:
                    //context.Call(new InstallAppDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    return await stepContext.BeginDialogAsync(
                        nameof(InstallAppDialog),
                        cancellationToken);
                case ResetPasswordOption:
                    //context.Call(new ResetPasswordDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    return await stepContext.BeginDialogAsync(
                        nameof(ResetPasswordDialog),
                        cancellationToken);
                case LocalAdminOption:
                    //context.Call(new LocalAdminDialog(), this.ResumeAfterOptionDialog);
                    //break;
                    return await stepContext.BeginDialogAsync(
                        nameof(LocalAdminDialog),
                        cancellationToken);
            }

            // We shouldn't get here, but fail gracefully if we do.
            await stepContext.Context.SendActivityAsync(
                "I don't recognize that option.",
                cancellationToken: cancellationToken);
            // Continue through to the next step without starting a child dialog.
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ResumeAfterAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                //var message = await userReply;
                var message = stepContext.Context.Activity;

                int ticketNumber = new Random().Next(0, 20000);
                //await context.PostAsync($"Thank you for using the Helpdesk Bot. Your ticket number is {ticketNumber}.");
                await stepContext.Context.SendActivityAsync(
                    $"Thank you for using the Helpdesk Bot. Your ticket number is {ticketNumber}.",
                    cancellationToken: cancellationToken);

                //context.Done(ticketNumber);
            }
            catch (Exception ex)
            {
                // await context.PostAsync($"Failed with message: {ex.Message}");
                await stepContext.Context.SendActivityAsync(
                    $"Failed with message: {ex.Message}",
                    cancellationToken: cancellationToken);

                // In general resume from task after calling a child dialog is a good place to handle exceptions
                // try catch will capture exceptions from the bot framework awaitable object which is essentially "userReply"
                logger.Error(ex);
            }

            // Replace on the stack the current instance of the waterfall with a new instance,
            // and start from the top.
            return await stepContext.ReplaceDialogAsync(
                "choiceswaterfall",
                cancellationToken: cancellationToken);
        }
    }
}
