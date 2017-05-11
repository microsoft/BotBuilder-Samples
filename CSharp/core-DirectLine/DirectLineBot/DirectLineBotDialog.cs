namespace DirectLineBot
{
    using System;
    using System.Collections.Generic;
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
            reply.Attachments = new List<Attachment>();

            switch (message.Text.ToLower())
            {
                case "show me a hero card":
                    reply.Text = $"Sample message with a HeroCard attachment";

                    var heroCardAttachment = new HeroCard
                    {
                        Title = "Sample Hero Card",
                        Text = "Displayed in the DirectLine client"
                    }.ToAttachment();

                    reply.Attachments.Add(heroCardAttachment);
                    break;
                case "send me a botframework image":
                    
                    reply.Text = $"Sample message with an Image attachment";

                    var imageAttachment = new Attachment()
                    {
                        ContentType = "image/png",
                        ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
                    };

                    reply.Attachments.Add(imageAttachment);

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
