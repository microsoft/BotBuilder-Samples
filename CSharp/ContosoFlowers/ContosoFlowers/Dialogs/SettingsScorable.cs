namespace ContosoFlowers.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;

    public class SettingsScorable : IScorable<double>
    {
        private readonly IDialogStack stack;
        private readonly IContosoFlowersDialogFactory dialogFactory;

        public SettingsScorable(IDialogStack stack, IContosoFlowersDialogFactory dialogFactory)
        {
            SetField.NotNull(out this.stack, nameof(stack), stack);
            SetField.NotNull(out this.dialogFactory, nameof(dialogFactory), dialogFactory);
        }

        public async Task<object> PrepareAsync<Item>(Item item, CancellationToken token)
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

        public bool TryScore(object state, out double score)
        {
            bool matched = state != null;
            score = matched ? 1.0 : double.NaN;
            return matched;
        }

        public async Task PostAsync<Item>(Item item, object state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                var settingsDialog = new SettingsDialog(this.dialogFactory);

                // wrap it with an additional dialog that will restart the wait for
                // messages from the user once the child dialog has finished
                var interruption = settingsDialog.Void<object, IMessageActivity>();
                
                // put the interrupting dialog on the stack
                this.stack.Call(interruption, null);

                // start running the interrupting dialog
                await this.stack.PollAsync(token);
            }
        }
    }
}
