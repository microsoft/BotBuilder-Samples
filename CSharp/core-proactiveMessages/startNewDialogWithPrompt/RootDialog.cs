using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace startNewDialogWithPrompt
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

            //Prepare the timer to simulate a background/asynchonous process
            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            var url = HttpContext.Current.Request.Url;
            //We echo the message regardless
            await context.PostAsync("Hello. In a few seconds I'll interrupt this dialog and bring another one with a prompt. You can also make me send a message by accessing: " +
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