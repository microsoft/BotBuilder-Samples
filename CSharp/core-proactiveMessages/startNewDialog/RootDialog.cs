using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace startNewDialog
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        [NonSerialized]
        Timer t;
        
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var conversationReference = message.ToConversationReference();
            ConversationStarter.conversationReference = JsonConvert.SerializeObject(conversationReference);

            //We will start a timer to fake a background service that will trigger the proactive message

            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            var url = HttpContext.Current.Request.Url;
            await context.PostAsync("Hey there, I'm going to interrupt our conversation and start a survey in a few seconds. You can also make me send a message by accessing: " +
                    url.Scheme + "://" + url.Host + ":" + url.Port + "/api/CustomWebApi"); 
            context.Wait(MessageReceivedAsync);
        }

        public void timerEvent(object target)
        {
            t.Dispose();
            ConversationStarter.Resume(); //We don't need to wait for this, just want to start the interruption here
        }
    }
}