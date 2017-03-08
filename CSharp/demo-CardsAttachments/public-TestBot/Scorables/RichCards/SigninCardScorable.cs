namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class SigninCardScorable : RichCardScorable
    {
        public SigninCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Signin";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new SigninCard
                {
                    Text = "BotFramework Sign-in Card",
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
                }.ToAttachment()
            };
        }
    }
}