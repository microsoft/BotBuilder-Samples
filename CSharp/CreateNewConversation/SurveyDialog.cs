namespace CreateNewConversationBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public sealed class SurveyDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var form = new FormDialog<Survey>(new Survey(), BuildSurveyForm, FormOptions.PromptInStart);
            context.Call(form, this.OnSurveyCompleted);
        }

        private static IForm<Survey> BuildSurveyForm()
        {
            return new FormBuilder<Survey>()
                .AddRemainingFields()
                .Build();
        }

        private async Task OnSurveyCompleted(IDialogContext context, IAwaitable<Survey> result)
        {
            try
            {
                var survey = await result;

                await context.PostAsync($"Got it... {survey.Name} you've been programming for {survey.YearsCoding} years and use {survey.Language}.");
            }
            catch (FormCanceledException<Survey> e)
            {
                string reply;

                if (e.InnerException == null)
                {
                    reply = "You have canceled the survey";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {e.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }

            context.Done(string.Empty);
        }   
    }
}
