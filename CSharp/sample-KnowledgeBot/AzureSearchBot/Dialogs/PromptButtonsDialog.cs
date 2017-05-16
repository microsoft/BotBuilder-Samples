using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace AzureSearchBot.Dialogs
{
    [Serializable]
    //This is actually Root Dialog of this bot, but I named PromptButtons Dialog becuase I want to set similar name in node.js sample.
    public class PromptButtonsDialog : IDialog<object>
    {
        private const string ExplorerOption = "Musician Explorer";
        private const string SearchOption =  "Musician Search";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //Show options whatever users chat
            PromptDialog.Choice(context, this.AfterMenuSelection, new List<string>() {ExplorerOption , SearchOption}, "How would you like to explore the classical music bot?");
        }

        //After users select option, Bot call other dialogs
        private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            switch(optionSelected)
            {
                case ExplorerOption:
                    context.Call(new MusicianExplorerDialog(), ResumeAfterOptionDialog);
                    break;
                case SearchOption:
                    context.Call(new MusicianSearchDialog(), ResumeAfterOptionDialog);
                    break;
            }

        }

        //This function is called after each dialog process is done
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            //This means  MessageRecievedAsync function of this dialog (PromptButtonsDialog) will receive users' messeges
            context.Wait(MessageReceivedAsync);
        }
    }
}