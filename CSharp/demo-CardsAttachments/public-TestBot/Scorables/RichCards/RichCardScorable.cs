namespace TestBot.Scorables
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public abstract class RichCardScorable : ExtractCodeScorable
    {
        private const string CSharpSamplesRoot = "https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/";
        private const string NodeJsSamplesRoot = "https://github.com/Microsoft/BotBuilder-Samples/tree/master/Node/";
        private const string RichCardsSample = "cards-RichCards";
        private const string CarouselSample = "cards-CarouselCards";
        private const string RichCardsText = "Rich Cards";
        private const string CarouselText = "Carousel";

        public RichCardScorable(IBotToUser botToUser, IBotData botData) : base(botToUser, botData)
        {
        }

        protected abstract IList<Attachment> GetCardAttachments();

        protected override async Task PostAsync(IActivity item, bool state, CancellationToken token)
        {
            await base.PostAsync(item, state, token);

            var message = this.BotToUser.MakeMessage();

            message.Attachments = this.GetCardAttachments();

            var sample = RichCardsSample;
            var text = RichCardsText;

            // should display carousel?
            if (message.Attachments.Count() > 1)
            {
                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                sample = CarouselSample;
                text = CarouselText;
            }

            await this.BotToUser.PostAsync(message);

            var moreMessage = this.BotToUser.MakeMessage();

            moreMessage.Text = $"To know more about {text} check these [C#]({CSharpSamplesRoot + sample}) & [NodeJs]({NodeJsSamplesRoot + sample}) samples. Type **{Constants.JsonTrigger}** to view attachment details;  or type **{Constants.CSharpTrigger}** or **{Constants.NodeJsTrigger}** for the source code that generated this card.";

            await this.BotToUser.PostAsync(moreMessage);
        }
    }
}