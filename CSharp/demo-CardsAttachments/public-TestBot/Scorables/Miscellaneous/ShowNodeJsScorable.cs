namespace TestBot.Scorables
{
    using Microsoft.Bot.Builder.Dialogs.Internals;

    public class ShowNodeJsScorable : ShowPrivateDataScorable
    {
        public ShowNodeJsScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string DataKey
        {
            get
            {
                return Constants.LastNodeJsKey;
            }
        }

        public override string NotAvailableMessage
        {
            get
            {
                return "Sorry, Node code not available!";
            }
        }

        public override string Trigger
        {
            get
            {
                return Constants.NodeJsTrigger;
            }
        }
    }
}