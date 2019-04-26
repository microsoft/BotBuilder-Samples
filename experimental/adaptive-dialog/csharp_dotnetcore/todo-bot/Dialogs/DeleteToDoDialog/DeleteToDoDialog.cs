using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteToDoDialog : ComponentDialog
    {
        public DeleteToDoDialog()
            : base(nameof(DeleteToDoDialog))
        {
            // Create instance of adaptive dialog. 
            var DeleteToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    new SendActivity("Add todos! :)")
                }
            };

            // Add all child dialogs


            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(DeleteToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
