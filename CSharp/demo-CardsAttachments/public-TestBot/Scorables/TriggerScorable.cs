namespace TestBot.Scorables
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Builder.Scorables.Internals;
    using Microsoft.Bot.Connector;

    public abstract class TriggerScorable : ScorableBase<IActivity, bool, double>
    {
        protected readonly IBotToUser BotToUser;
        protected readonly IBotData BotData;

        public TriggerScorable(IBotToUser botToUser, IBotData botData)
        {
            SetField.NotNull(out this.BotToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.BotData, nameof(botData), botData);
        }

        public abstract string Trigger { get; }

        protected override Task DoneAsync(IActivity item, bool state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, bool state)
        {
            return state ? 1 : 0;
        }

        protected override bool HasScore(IActivity item, bool state)
        {
            return state;
        }

        protected override Task<bool> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item.AsMessageActivity();

            if (message == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(message.Text.ToLowerInvariant().Contains(this.Trigger.ToLowerInvariant()));
        }
    }
}