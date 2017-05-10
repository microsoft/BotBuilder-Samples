namespace CreateNewConversationBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Microsoft.Rest;

    public static class SurveyTriggerer
    {
        public static async Task StartSurvey(ConversationReference conversationReference, CancellationToken token)
        {
            var container = WebApiApplication.FindContainer();

            // the ConversationReference has the "key" necessary to resume the conversation
            var message = conversationReference.GetPostToBotMessage();

            ConnectorClient client = new ConnectorClient(new Uri(message.ServiceUrl));

            try
            {
                var conversation = await client.Conversations.CreateDirectConversationAsync(message.Recipient, message.From);
                message.Conversation.Id = conversation.Id;
            }
            catch (HttpOperationException ex)
            {
                var reply = message.CreateReply();
                reply.Text = ex.Message;

                await client.Conversations.SendToConversationAsync(reply);

                return;
            }

            // we instantiate our dependencies based on an IMessageActivity implementation
            using (var scope = DialogModule.BeginLifetimeScope(container, message))
            {
                // find the bot data interface and load up the conversation dialog state
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(token);

                // resolve the dialog task
                IDialogTask task = scope.Resolve<IDialogTask>();

                // make a dialog to push on the top of the stack
                var child = scope.Resolve<SurveyDialog>();

                // wrap it with an additional dialog that will restart the wait for
                // messages from the user once the child dialog has finished
                var interruption = child.Void<object, IMessageActivity>();

                try
                {
                    // put the interrupting dialog on the stack
                    task.Call(interruption, null);

                    // start running the interrupting dialog
                    await task.PollAsync(token);
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
