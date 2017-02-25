namespace ContosoFlowers.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;

    public class SettingsScorable : IScorable<IActivity, double>
    {
        private readonly IDialogTask task;
        private readonly IContosoFlowersDialogFactory dialogFactory;

        public SettingsScorable(IDialogTask task, IContosoFlowersDialogFactory dialogFactory)
        {
            SetField.NotNull(out this.task, nameof(task), task);
            SetField.NotNull(out this.dialogFactory, nameof(dialogFactory), dialogFactory);
        }

        public async Task<object> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("settings", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }

        public bool HasScore(IActivity item, object state)
        {
            return state != null;
        }

        public double GetScore(IActivity item, object state)
        {
            bool matched = state != null;
            var score = matched ? 1.0 : double.NaN;
            return score;
        }

        public async Task PostAsync(IActivity item, object state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                var settingsDialog = new SettingsDialog(this.dialogFactory);

                // wrap it with an additional dialog that will restart the wait for
                // messages from the user once the child dialog has finished
                var interruption = settingsDialog.Void<object, IMessageActivity>();

                // put the interrupting dialog on the stack
                this.task.Call(interruption, null);

                // start running the interrupting dialog
                await this.task.PollAsync(token);
            }
        }

        public Task DoneAsync(IActivity item, object state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
