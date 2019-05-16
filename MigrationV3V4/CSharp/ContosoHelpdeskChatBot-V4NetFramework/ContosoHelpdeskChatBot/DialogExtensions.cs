using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ContosoHelpdeskChatBot
{
    public static class DialogExtensions
    {
        public static async Task Run(this Dialog dialog, ITurnContext turnContext, IStatePropertyAccessor<DialogState> accessor, CancellationToken cancellationToken)
        {
            var dialogSet = new DialogSet(accessor);
            dialogSet.Add(dialog);

            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);

            // Handle 'cancel' interruption
            if (turnContext.Activity.Text.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                Activity reply = turnContext.Activity.CreateReply($"Ok restarting conversation.");
                await turnContext.SendActivityAsync(reply);
                await dialogContext.CancelAllDialogsAsync();
            }
            
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(dialog.Id, null, cancellationToken);
            }
        }
    }
}
