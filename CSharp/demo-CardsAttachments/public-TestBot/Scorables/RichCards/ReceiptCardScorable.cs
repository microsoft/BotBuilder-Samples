namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class ReceiptCardScorable : RichCardScorable
    {
        public ReceiptCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        public override string Trigger
        {
            get
            {
                return "Receipt";
            }
        }

        protected override IList<Attachment> GetCardAttachments()
        {
            return new List<Attachment>
            {
                new ReceiptCard
                {
                    Title = "John Doe",
                    Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                    Items = new List<ReceiptItem>
                    {
                        new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
                        new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
                    },
                    Tax = "$ 7.50",
                    Total = "$ 90.95",
                    Buttons = new List<CardAction>
                    {
                        new CardAction(
                            ActionTypes.OpenUrl,
                            "More information",
                            "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
                            "https://azure.microsoft.com/en-us/pricing/")
                    }
                }.ToAttachment()
            };
        }
    }
}