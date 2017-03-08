namespace TestBot.Scorables
{
    using Microsoft.Bot.Builder.Dialogs.Internals;

    public class ShowJsonScorable : ShowPrivateDataScorable
    {
        public ShowJsonScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string DataKey
        {
            get
            {
                return Constants.LastJsonKey;
            }
        }

        public override string NotAvailableMessage
        {
            get
            {
                return "Sorry, JSON not available!";
            }
        }

        public override string Trigger
        {
            get
            {
                return Constants.JsonTrigger;
            }
        }
    }
}