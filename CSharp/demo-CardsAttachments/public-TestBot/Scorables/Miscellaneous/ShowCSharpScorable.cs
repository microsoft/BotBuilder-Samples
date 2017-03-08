namespace TestBot.Scorables
{
    using Microsoft.Bot.Builder.Dialogs.Internals;

    public class ShowCSharpScorable : ShowPrivateDataScorable
    {
        public ShowCSharpScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string DataKey
        {
            get
            {
                return Constants.LastCSharpKey;
            }
        }

        public override string NotAvailableMessage
        {
            get
            {
                return "Sorry, C# code not available!";
            }
        }

        public override string Trigger
        {
            get
            {
                return Constants.CSharpTrigger;
            }
        }
    }
}