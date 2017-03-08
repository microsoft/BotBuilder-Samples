namespace TestBot.Scorables
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public abstract class ShowPrivateDataScorable : TriggerScorable
    {
        private const string CodeTemplate = "~~~~\n{0}\n~~~~";

        public ShowPrivateDataScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public abstract string DataKey { get; }

        public abstract string NotAvailableMessage { get; }

        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            var data = default(string);

            this.BotData.PrivateConversationData.TryGetValue(this.DataKey, out data);

            var reply = this.BotToUser.MakeMessage();

            reply.Text = string.IsNullOrWhiteSpace(data) ? this.NotAvailableMessage : string.Format(CodeTemplate, data);

            await this.BotToUser.PostAsync(reply);
        }
    }
}