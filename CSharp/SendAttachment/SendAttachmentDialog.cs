namespace SendAttachmentBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    internal class SendAttachmentDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var replyMessage = context.MakeMessage();

            // The Attachments property allows you to send and receive images and other content
            replyMessage.Attachments = new List<Attachment>()
            {
                new Attachment()
                {
                    ContentUrl = "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png",
                    ContentType = "image/png",
                    Name = "BotFrameworkOverview.png"
                }
            };

            await context.PostAsync(replyMessage);

            context.Wait(this.MessageReceivedAsync);
        }
    }
}