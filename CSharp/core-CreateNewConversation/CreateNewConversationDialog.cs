namespace CreateNewConversationBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class CreateNewConversationDialog : IDialog<object>
    {
        private readonly ISurveyService surveyService;

        public CreateNewConversationDialog(ISurveyService surveyService)
        {
            SetField.NotNull(out this.surveyService, nameof(surveyService), surveyService);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync("You've been invited to a survey! It will start in a few seconds...");

            await this.surveyService.QueueSurveyAsync();

            context.Wait(this.MessageReceivedAsync);
        }
    }
}