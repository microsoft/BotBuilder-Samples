namespace DirectLineBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class DirectLineBotDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var reply = context.MakeMessage();

            switch (message.Text.ToLower())
            {
                case "show me a hero card":
                    reply.Text = $"Sample message with ChannelData using the BotBuilder Hero's card";

                    var heroCardAttachment = new HeroCard
                    {
                        Title = "Sample Hero Card",
                        Text = "Displayed in the DirectLine client"
                    }.ToAttachment();

                    // Not using a anonymous object due to #998 (https://github.com/Microsoft/BotBuilder/issues/998)
                    reply.ChannelData = heroCardAttachment;

                    break;
                case "send me a botframework image":
                    
                    reply.Text = $"Sample message with ChannelData using the BotBuilder Attachment structure";

                    var imageAttachment = new Attachment()
                    {
                        ContentType = "image/png",
                        ContentUrl = "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png",
                    };

                    // Not using a anonymous object due to #998 (https://github.com/Microsoft/BotBuilder/issues/998)
                    reply.ChannelData = imageAttachment;

                    break;
                default:
                    reply.Text = $"You said '{message.Text}'";
                    break;
            }

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }
    }
}
