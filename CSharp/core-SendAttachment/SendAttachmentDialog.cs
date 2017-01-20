namespace SendAttachmentBot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    internal class SendAttachmentDialog : IDialog<object>
    {
        private const string ShowInlineAttachment = "(1) Show inline attachment";
        private const string ShowUploadedAttachment = "(2) Show uploaded attachment";
        private const string ShowInternetAttachment = "(3) Show internet attachment";

        private readonly IDictionary<string, string> options = new Dictionary<string, string> {
            { "1", ShowInlineAttachment },
            { "2", ShowUploadedAttachment },
            { "3", ShowInternetAttachment }
        };

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = "Welcome, here you can see attachment alternatives:";

            await context.PostAsync(welcomeMessage);

            await this.DisplayOptionsAsync(context, null);
        }

        public Task DisplayOptionsAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            PromptDialog.Choice<string>(
                context,
                this.ProcessSelectedOptionAsync,
                this.options.Keys,
                "What sample option would you like to see?",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.PerLine,
                this.options.Values);

            return Task.CompletedTask;
        }

        public async Task ProcessSelectedOptionAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;

            var replyMessage = context.MakeMessage();
    
            var attachmentInfo = default(KeyValuePair<string, string>);
            switch(message)
            {
                case "1":
                    {
                        attachmentInfo = this.GetInlineAttachmentInfo();
                        break;
                    }
                case "2":
                    {
                        attachmentInfo = await this.GetUploadedAttachmentInfoAsync(replyMessage.ServiceUrl, replyMessage.Conversation.Id);
                        break;
                    }
                case "3":
                    {
                        attachmentInfo = this.GetInternetAttachmentInfo();
                        break;
                    }
            }

            // The Attachments property allows you to send and receive images and other content
            replyMessage.Attachments = new List<Attachment>()
            {
                new Attachment()
                {
                    ContentUrl = attachmentInfo.Value,
                    ContentType = "image/png",
                    Name = attachmentInfo.Key
                }
            };

            await context.PostAsync(replyMessage);

            await this.DisplayOptionsAsync(context, null);
        }

        private KeyValuePair<string, string> GetInlineAttachmentInfo()
        {
            var imagePath = HttpContext.Current.Server.MapPath("~/images/small-image.png");

            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new KeyValuePair<string, string>(
                "small-image.png",
                $"data:image/png;base64,{imageData}");
        }

        private async Task<KeyValuePair<string, string>> GetUploadedAttachmentInfoAsync(string serviceUrl, string conversationId)
        {
            var imagePath = HttpContext.Current.Server.MapPath("~/images/big-image.png");

            using (var connector = new ConnectorClient(new Uri(serviceUrl)))
            {
                var attachments = new Attachments(connector);
                var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                    conversationId,
                    new AttachmentData
                    {
                        Name = "big-image.png",
                        OriginalBase64 = File.ReadAllBytes(imagePath),
                        Type = "image/png"
                    });

                var attachmentUri = attachments.GetAttachmentUri(response.Id);

                // Typo bug in current assembly version '.Replace("{vieWId}", Uri.EscapeDataString(viewId))'
                // TODO: remove this line when replacement Bug is fixed on future releases
                attachmentUri = attachmentUri.Replace("{viewId}", "original");

                return new KeyValuePair<string, string>(
                    "big-image.png",
                    attachmentUri);
            }
        }

        private KeyValuePair<string, string> GetInternetAttachmentInfo()
        {
            return new KeyValuePair<string, string>(
                "BotFrameworkOverview.png", 
                "https://docs.botframework.com/en-us/images/faq-overview/botframework_overview_july.png");
        }
    }
}