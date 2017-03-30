namespace Azure_Bot_Generic_CSharp
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Dialogs;
    using Models;

    [Serializable]
    public class ShareButtonDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            //create a reply message
            var reply = context.MakeMessage();
            //create a channel data object to act as a facebook share button
            reply.ChannelData = new FacebookChannelData()
            {
                Attachment = new FacebookAttachment()
                {
                    Payload = new FacebookGenericTemplate()
                    {
                        Elements = new object[]
                        {
                            new FacebookGenericTemplateContent()
                            {
                                Buttons = new[]
                                {
                                    new FacebookShareButton()
                                }
                            }
                        }
                    }
                }
            };

            //send message
            await context.PostAsync(reply);

            var reply2 = context.MakeMessage();
            reply2.Text = "This is a message after the Share Button template.";
            await context.PostAsync(reply2);
            //wait for more messages to be sent here
            context.Wait(MessageReceivedAsync);
        }
    }
}