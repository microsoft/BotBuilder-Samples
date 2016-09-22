namespace CreateNewConversationBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public static class SurveyTriggerer
    {
        public static async Task StartSurvey(ResumptionCookie cookie, CancellationToken token)
        {
            var container = WebApiApplication.FindContainer();

            // the ResumptionCookie has the "key" necessary to resume the conversation
            var message = cookie.GetMessage();

            // we instantiate our dependencies based on an IMessageActivity implementation
            using (var scope = DialogModule.BeginLifetimeScope(container, message))
            {
                // find the bot data interface and load up the conversation dialog state
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(token);

                // resolve the dialog stack
                IDialogStack stack = stack = scope.Resolve<IDialogStack>();

                // make a dialog to push on the top of the stack
                var child = scope.Resolve<SurveyDialog>();

                // wrap it with an additional dialog that will restart the wait for
                // messages from the user once the child dialog has finished
                var interruption = child.Void<object, IMessageActivity>();

                try
                {
                    // put the interrupting dialog on the stack
                    stack.Call(interruption, null);

                    // start running the interrupting dialog
                    await stack.PollAsync(token);
                }
                finally
                {
                    // save out the conversation dialog state
                    await botData.FlushAsync(token);
                }
            }
        }
    }
}
