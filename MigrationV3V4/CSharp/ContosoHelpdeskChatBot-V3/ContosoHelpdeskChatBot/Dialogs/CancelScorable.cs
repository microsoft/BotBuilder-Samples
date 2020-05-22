namespace ContosoHelpdeskChatBot.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Scorables.Internals;
    using Autofac;

#pragma warning disable 1998

    public class CancelScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public CancelScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            var message = activity as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var activity = (Activity)item;
            var reply = activity.CreateReply($"Ok restarting conversation.");

            //Refer to Connector service
            //https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-connector
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            await connector.Conversations.ReplyToActivityAsync(reply);

            this.task.Reset();
        }
        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
